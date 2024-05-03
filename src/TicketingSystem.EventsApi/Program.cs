using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using TicketingSystem.Api;
using TicketingSystem.BusinessLogic;
using TicketingSystem.BusinessLogic.Mapper;

namespace TicketingSystem.EventsApi
{
    public class Program : BaseProgram
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

            builder.Services.AddBusinessLogicServices(connectionString, databaseName);

            builder.Services.AddSingleton(SetupMapper());

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(config =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);

                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Events API",
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
    }
}
