using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class SectionRepositoryTests : GenericMongoRepositoryTestsBase<Section>
    {
        public SectionRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<Section> CreateRepository(IMongoDbFactory factory)
            => new SectionRepository(factory);
    }
}