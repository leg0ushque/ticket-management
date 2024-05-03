using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using TicketingSystem.BusinessLogic.Mapper;

namespace TicketingSystem.Api
{
    public class BaseProgram
    {
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
