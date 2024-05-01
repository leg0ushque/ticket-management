using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.DataAccess;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic
{
    public static class DiExtension
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services,
            string connectionString, string databaseName)
        {
            return services
                .AddDataAccessServices(connectionString, databaseName)
                .RegisterServices();
        }

        private static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ICartItemService, CartItemService>()
                .AddTransient<IEventService, EventService>()
                .AddTransient<IEventSectionService, EventSectionService>()
                .AddTransient<IEventSeatService, EventSeatService>()
                .AddTransient<IPaymentService, PaymentService>()
                .AddTransient<ISectionService, SectionService>()
                .AddTransient<ITicketService, TicketService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IVenueService, VenueService>();
        }
    }
}
