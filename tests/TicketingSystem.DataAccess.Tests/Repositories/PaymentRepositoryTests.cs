using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.UnitTests
{
    public class PaymentRepositoryTests : GenericMongoRepositoryTestsBase<Payment>
    {
        public PaymentRepositoryTests()
        {
            Setup();
        }

        public override IMongoRepository<Payment> CreateRepository(IMongoDbFactory factory)
            => new PaymentRepository(factory);
    }
}