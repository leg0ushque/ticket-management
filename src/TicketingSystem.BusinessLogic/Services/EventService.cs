using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventService(IMongoRepository<Event> repository, IMapper mapper)
        : GenericEntityService<Event, EventDto>(repository, mapper), IEventService
    {
    }
}
