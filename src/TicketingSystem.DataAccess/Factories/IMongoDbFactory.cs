using MongoDB.Driver;

namespace TicketingSystem.DataAccess.Factories
{
    public interface IMongoDbFactory
    {
        public IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}