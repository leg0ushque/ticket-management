using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using TicketingSystem.Messaging;
using TicketingSystem.Messaging.Options;
using TicketingSystem.Messaging.Producer;

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
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(services =>
            {
                services.AddOptions<KafkaOptions>()
                    .Bind(_configuration.GetSection(KafkaOptions.ConfigurationSection));

                services.AddTransient<KafkaConfigurationProvider>();
                services.AddTransient<IProducerProvider, KafkaProducerProvider>();
                services.AddTransient<IKafkaProducer, KafkaProducer>();
            });

            return hostBuilder;
        }
    }
}
