using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class UserRepositoryTests : GenericMongoRepositoryTestsBase<User>
    {
        public UserRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<User> CreateRepository(IMongoDbFactory factory)
            => new UserRepository(factory);
    }
}