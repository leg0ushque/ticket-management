
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using TicketingSystem.BusinessLogic;
using TicketingSystem.BusinessLogic.Mapper;

namespace TicketingSystem.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? throw new ArgumentException("ENV VAR");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.{env}.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            builder.Services.AddBusinessLogicServices(connectionString, databaseName);

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
                mc.AddProfile(new WebApiMappingProfile());
            });

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
