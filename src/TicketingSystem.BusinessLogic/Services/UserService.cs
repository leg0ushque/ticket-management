using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class UserService : GenericEntityService<User, VenueDto>, IUserService
    {
        public UserService(IMongoRepository<User> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
