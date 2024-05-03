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
    public class VenueService : GenericEntityService<Venue, VenueDto>, IVenueService
    {
        private readonly IMongoRepository<Section> _sectionRepository;

        public VenueService(IMongoRepository<Venue> venueRepository, IMongoRepository<Section> sectionRepository, IMapper mapper) : base(venueRepository, mapper)
        {
            _sectionRepository = sectionRepository;
        }

        public async Task<List<Section>> GetVenueSectionsAsync(string venueId, CancellationToken cancellationToken = default)
        {
            var venue = await _repository.GetByIdAsync(venueId, cancellationToken);

            if (venue == null)
            {
                throw new BusinessLogicException("Venue wasn't found", code: ErrorCode.NotFound);
            }

            return await _sectionRepository.FilterAsync(x => x.VenueId == venueId, cancellationToken);
        }
    }
}
