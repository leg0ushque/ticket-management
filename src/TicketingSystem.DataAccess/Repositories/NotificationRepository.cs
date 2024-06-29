using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;

namespace TicketingSystem.DataAccess.Repositories
{
    public class NotificationRepository : GenericMongoRepository<Notification>
    {
        public NotificationRepository(IMongoDbFactory mongoDbFactory) : base(mongoDbFactory)
        { }

        public override string _collectionName => "Notifications";
    }
}
