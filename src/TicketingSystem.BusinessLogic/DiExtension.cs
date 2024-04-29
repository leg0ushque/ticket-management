using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.BusinessLogic.Validators;
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
                .RegisterValidators()
                .RegisterServices();
        }

        private static IServiceCollection RegisterValidators(this IServiceCollection services)
        {
            return services
                .AddTransient<IValidator<CartItemDto>, CartItemValidator>()
                .AddTransient<IValidator<EventSeatDto>, EventSeatValidator>()
                .AddTransient<IValidator<EventSectionDto>, EventSectionValidator>()
                .AddTransient<IValidator<EventDto>, EventValidator>()
                .AddTransient<IValidator<SectionDto>, SectionValidator>()
                .AddTransient<IValidator<TicketDto>, TicketValidator>()
                .AddTransient<IAsyncValidator<UserDto>, UserValidator>()
                .AddTransient<IValidator<VenueDto>, VenueValidator>();

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
