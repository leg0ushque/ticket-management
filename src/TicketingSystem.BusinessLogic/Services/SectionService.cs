using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class SectionService : GenericEntityService<Section, SectionDto>, ISectionService
    {
        public SectionService(IMongoRepository<Section> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
