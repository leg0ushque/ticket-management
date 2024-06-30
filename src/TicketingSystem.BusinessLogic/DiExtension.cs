using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.BusinessLogic.Options;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.DataAccess;
using TicketingSystem.Messaging.Options;

namespace TicketingSystem.BusinessLogic
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services,
            string connectionString, string databaseName)
        {
            return services.AddDataAccessServices(connectionString, databaseName)
                .AddMemoryCache()
                .RegisterServices();
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IEventService, EventService>()
                .AddTransient<IEventSectionService, EventSectionService>()
                .AddTransient<IPaymentService, PaymentService>()
                .AddTransient<ISectionService, SectionService>()
                .AddTransient<ITicketService, TicketService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IVenueService, VenueService>()
                .AddTransient<INotificationService, NotificationService>();
        }
    }
}
