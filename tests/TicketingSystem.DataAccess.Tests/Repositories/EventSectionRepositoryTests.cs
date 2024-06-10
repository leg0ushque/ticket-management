using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class EventSectionRepositoryTests : GenericMongoRepositoryTestsBase<EventSection>
    {
        public EventSectionRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<EventSection> CreateRepository(IMongoDbFactory factory)
            => new EventSectionRepository(factory);
    }
}