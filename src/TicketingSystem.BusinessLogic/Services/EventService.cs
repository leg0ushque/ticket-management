using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventService : GenericEntityService<Event, EventDto>, IEventService
    {
        public EventService(IMongoRepository<Event> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
