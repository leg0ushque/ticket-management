using Amazon.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using TicketingSystem.BusinessLogic;
using TicketingSystem.Messaging;
using TicketingSystem.Messaging.Options;
using TicketingSystem.Messaging.Producer;
using TicketingSystem.NotificationHandlerApp.EmailProviders;
using TicketingSystem.NotificationHandlerApp.HttpClients;
using TicketingSystem.NotificationHandlerApp.Options;

namespace TicketingSystem.NotificationHandlerApp
{
    public static class Program
    {
        private static IConfiguration _configuration;

        public static void Main(string[] args)
        {
            var app = CreateHostBuilder(args).Build();
            Console.WriteLine($"Notifications handling started...");

            app.Run();

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? throw new ArgumentException("Missing ASPNETCORE_ENVIRONMENT env var");

            var config = SetupConfiguration(env);

            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IConfiguration>(config);

                services.AddOptions<MailJetOptions>()
                    .Bind(config.GetSection(MailJetOptions.ConfigurationKey));

                services.AddHttpClient<IMailHttpClient, MailJetHttpClient>(c =>
                {
                    c.BaseAddress = new Uri(config["MailJet:ApiBaseAddress"]);
                });

                services.AddHttpClient<IMailHttpClient, MailJetHttpClient>();
                services.AddSingleton<IEmailProvider, MailJetEmailProvider>();


                var connectionString = config.GetConnectionString("connectionString");
                var databaseName = config.GetSection("databaseName").Value;

                services.AddBusinessLogicServices(connectionString, databaseName);

                services.AddOptions<KafkaOptions>()
                    .Bind(_configuration.GetSection(KafkaOptions.ConfigurationKey));

                services.AddTransient<KafkaConfigurationProvider>();
                services.AddTransient<IProducerProvider, KafkaProducerProvider>();
                services.AddTransient<IKafkaProducer, KafkaProducer>();
                services.AddTransient<IMessageHandler, MessageHandler>();

                services.AddHostedService<KafkaConsumingHostedService>();
            });

            return hostBuilder;
        }

        public static IConfiguration SetupConfiguration(string env)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.{env}.json")
                .Build();
        }
    }
}
