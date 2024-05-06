using AutoMapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class VenueService(IMongoRepository<Venue> venueRepository, IMongoRepository<Section> sectionRepository, IMapper mapper)
        : GenericEntityService<Venue, VenueDto>(venueRepository, mapper), IVenueService
    {
        private readonly IMongoRepository<Section> _sectionRepository = sectionRepository;

        public async Task<List<Section>> GetVenueSectionsAsync(string venueId, CancellationToken cancellationToken = default)
        {
            var venue = await _repository.GetByIdAsync(venueId, cancellationToken);

            return venue == null
                ? throw new BusinessLogicException("Venue wasn't found", code: ErrorCode.NotFound)
                : await _sectionRepository.FilterAsync(x => x.VenueId == venueId, cancellationToken);
        }
    }
}
