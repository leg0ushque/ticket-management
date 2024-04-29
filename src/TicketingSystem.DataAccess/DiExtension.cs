using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DataAccess
{
    public static class DiExtension
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            return services
                .AddSingleton<IMongoDatabase>(database)
                .AddScoped(typeof(IMongoRepository<>), typeof(GenericMongoRepository<>));
        }
    }
}
