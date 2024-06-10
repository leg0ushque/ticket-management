using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class VenueRepositoryTests : GenericMongoRepositoryTestsBase<Venue>
    {
        public VenueRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<Venue> CreateRepository(IMongoDbFactory factory)
            => new VenueRepository(factory);
    }
}