using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class EventSeatRepository : GenericMongoRepository<EventSeat>
    {
        public EventSeatRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "EventSeats";
    }
}
