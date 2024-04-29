using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class VenueService : GenericEntityService<Venue, VenueDto>, IVenueService
    {
        public VenueService(IMongoRepository<Venue> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
