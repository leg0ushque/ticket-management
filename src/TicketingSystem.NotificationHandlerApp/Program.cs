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
using TicketingSystem.Messaging.Consumer;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using TicketingSystem.DataAccess.Factories;
using Serilog;
using Serilog.Events;

namespace TicketingSystem.NotificationHandlerApp
{
    public static class Program
    {
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

                services.AddSingleton<IMongoDbFactory>(new MongoDbFactory(connectionString, databaseName));
                services.AddTransient<IMongoRepository<Notification>, NotificationRepository>();
                services.AddTransient<INotificationService, NotificationService>();

                services.AddOptions<KafkaOptions>()
                    .Bind(config.GetSection(KafkaOptions.ConfigurationKey));

                services.AddTransient<KafkaConfigurationProvider>();
                services.AddTransient<IConsumerProvider, KafkaConsumerProvider>();
                services.AddTransient<IKafkaConsumer, KafkaConsumer>();
                services.AddTransient<IMessageHandler, MessageHandler>();

                services.AddHostedService<KafkaConsumingHostedService>();

                var loggerOutputTemplate =
                    "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

                services.AddSerilog(new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.RollingFile(@"Logs\Info-{Date}.log", outputTemplate: loggerOutputTemplate))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.RollingFile(@"Logs\Debug-{Date}.log", outputTemplate: loggerOutputTemplate))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.RollingFile(@"Logs\Warning-{Date}.log", outputTemplate: loggerOutputTemplate))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.RollingFile(@"Logs\Error-{Date}.log", outputTemplate: loggerOutputTemplate))
                    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.RollingFile(@"Logs\Fatal-{Date}.log", outputTemplate: loggerOutputTemplate))
                    .WriteTo.RollingFile(@"Logs\All-{Date}.log", outputTemplate: loggerOutputTemplate)
                    .CreateLogger());
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
