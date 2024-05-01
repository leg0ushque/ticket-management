using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class PaymentRepository : GenericMongoRepository<Payment>
    {
        public PaymentRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "Payments";
    }
}
