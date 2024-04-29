using AutoMapper;
using TicketingSystem.BusinessLogic;
using TicketingSystem.BusinessLogic.Mapper;

namespace TicketingSystem.VenuesApi
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

            builder.Services.AddBusinessLogicServices(connectionString, databaseName);

            builder.Services.SetupMapper();

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

        public static IServiceCollection SetupMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            return services.AddSingleton(mapper);
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
