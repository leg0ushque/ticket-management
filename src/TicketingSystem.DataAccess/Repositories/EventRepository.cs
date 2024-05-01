using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class EventRepository : GenericMongoRepository<Event>
    {
        public EventRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "Events";
    }
}
