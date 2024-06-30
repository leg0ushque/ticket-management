using MongoDB.Driver;

namespace TicketingSystem.DataAccess.Factories
{
    public interface IMongoDbFactory
    {
        public IMongoClient Client { get; }
        public IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}