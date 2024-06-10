using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class TicketRepositoryTests : GenericMongoRepositoryTestsBase<Ticket>
    {
        public TicketRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<Ticket> CreateRepository(IMongoDbFactory factory)
            => new TicketRepository(factory);
    }
}