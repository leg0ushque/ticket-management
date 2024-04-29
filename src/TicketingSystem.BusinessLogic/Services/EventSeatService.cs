using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSeatService : GenericEntityService<EventSeat, EventSeatDto>, IEventSeatService
    {
        public EventSeatService(IMongoRepository<EventSeat> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
