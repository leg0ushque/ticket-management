using MongoDB.Driver;

namespace TicketingSystem.DataAccess.Factories
{
    public class MongoDbFactory : IMongoDbFactory
    {
        private readonly string _databaseName;

        private readonly IMongoClient _client;

        public MongoDbFactory(string connectionString, string databaseName)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            _client = new MongoClient(settings);
            _databaseName = databaseName;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionNme)
        {
            return _client.GetDatabase(_databaseName).GetCollection<T>(collectionNme);
        }
    }
}
