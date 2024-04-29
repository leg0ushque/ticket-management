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
                .AddTransient<IService<CartItemDto>, GenericEntityService<CartItem, CartItemDto>>()
                .AddTransient<IService<EventDto>, GenericEntityService<Event, EventDto>>()
                .AddTransient<IService<EventSectionDto>, GenericEntityService<EventSection, EventSectionDto>>()
                .AddTransient<IService<EventSeatDto>, GenericEntityService<EventSeat, EventSeatDto>>()
                .AddTransient<IService<PaymentDto>, GenericEntityService<Payment, PaymentDto>>()
                .AddTransient<IService<EventSeatDto>, GenericEntityService<EventSeat, EventSeatDto>>()
                .AddTransient<IService<SectionDto>, GenericEntityService<Section, SectionDto>>()
                .AddTransient<IService<TicketDto>, GenericEntityService<Ticket, TicketDto>>()
                .AddTransient<IService<UserDto>, GenericEntityService<User, UserDto>>()
                .AddTransient<IService<VenueDto>, GenericEntityService<Venue, VenueDto>>();
        }
    }
}
