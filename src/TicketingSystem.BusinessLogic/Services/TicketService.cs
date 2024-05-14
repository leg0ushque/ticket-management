using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class TicketService : GenericEntityService<Ticket, TicketDto>, ITicketService
    {
        public TicketService(IMongoRepository<Ticket> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
