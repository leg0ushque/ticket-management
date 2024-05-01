using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class CartItemRepository : GenericMongoRepository<CartItem>
    {
        public CartItemRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "CartItems";
    }
}
