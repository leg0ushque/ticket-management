using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Mapper
{
    public class BusinessLogicMappingProfile : Profile
    {
        public BusinessLogicMappingProfile()
        {
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<EventRow, EventRowDto>().ReverseMap();
            CreateMap<EventSeat, EventSeatDto>().ReverseMap();
            CreateMap<EventSection, EventSectionDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Row, RowDto>().ReverseMap();
            CreateMap<Section, SectionDto>().ReverseMap();
            CreateMap<Ticket, TicketDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Venue, VenueDto>().ReverseMap();
        }
    }
}
