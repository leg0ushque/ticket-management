using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class EventRepositoryTests : GenericMongoRepositoryTestsBase<Event>
    {
        public EventRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<Event> CreateRepository(IMongoDbFactory factory)
            => new EventRepository(factory);
    }
}