using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;
using TicketingSystem.BusinessLogic;
using TicketingSystem.BusinessLogic.Mapper;
using TicketingSystem.BusinessLogic.Options;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Messaging;
using TicketingSystem.Messaging.Options;
using TicketingSystem.Messaging.Producer;

namespace TicketingSystem.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? throw new ArgumentException("Missing ASPNETCORE_ENVIRONMENT env var");

            var config = SetupConfiguration(env);
            builder.Services.AddSingleton<IConfiguration>(config);

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            builder.Services.AddOptions<CacheOptions>()
                .Bind(config.GetSection(CacheOptions.ConfigurationKey));

            builder.Services.AddBusinessLogicServices(connectionString, databaseName);

            builder.Services.AddSingleton(SetupMapper());

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddOptions<KafkaOptions>()
                .Bind(config.GetSection(KafkaOptions.ConfigurationKey));

            builder.Services.AddTransient<KafkaConfigurationProvider>();
            builder.Services.AddTransient<IProducerProvider, KafkaProducerProvider>();
            builder.Services.AddTransient<IKafkaProducer, KafkaProducer>();
            builder.Services.AddTransient<IKafkaNotificationService, KafkaNotificationService>();

            var loggerOutputTemplate =
                "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            var configuredLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(@"Logs\Info-{Date}.log", outputTemplate: loggerOutputTemplate))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-{Date}.log", outputTemplate: loggerOutputTemplate))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-{Date}.log", outputTemplate: loggerOutputTemplate))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(@"Logs\Error-{Date}.log", outputTemplate: loggerOutputTemplate))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-{Date}.log", outputTemplate: loggerOutputTemplate))
                .WriteTo.File(@"Logs\All-{Date}.log", outputTemplate: loggerOutputTemplate)
                .CreateLogger();

            builder.Services.AddSerilog(configuredLogger);

            builder.Services.AddSwaggerGen(config =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web API",
                    Version = "v1"
                });

                config.EnableAnnotations();
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
        public static IMapper SetupMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });

            return mapperConfig.CreateMapper();
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
