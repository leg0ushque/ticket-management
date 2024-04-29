using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSectionService : GenericEntityService<EventSection, EventSectionDto>, IEventSectionService
    {
        public EventSectionService(IMongoRepository<EventSection> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
