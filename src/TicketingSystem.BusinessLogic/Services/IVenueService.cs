using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IVenueService : IService<Venue, VenueDto>
    {
        public Task<List<SectionDto>> GetVenueSectionsAsync(string venueId, CancellationToken cancellationToken = default);
    }
}
