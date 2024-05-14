using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DataAccess
{
    public static class DiExtension
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, string connectionString, string databaseName)
        {
            return services
                .AddSingleton<IMongoDbFactory>(new MongoDbFactory(connectionString, databaseName))
                .AddTransient<IMongoRepository<Event>, EventRepository>()
                .AddTransient<IMongoRepository<EventSection>, EventSectionRepository>()
                .AddTransient<IMongoRepository<Payment>, PaymentRepository>()
                .AddTransient<IMongoRepository<Ticket>, TicketRepository>()
                .AddTransient<IMongoRepository<User>, UserRepository>()
                .AddTransient<IMongoRepository<Venue>, VenueRepository>()
                .AddTransient<IMongoRepository<Section>, SectionRepository>();
        }
    }
}
