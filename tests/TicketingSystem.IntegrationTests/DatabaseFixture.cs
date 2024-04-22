using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.IO;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.IntegrationTests
{
    public class DatabaseFixture
    {
        public IMongoRepository<Event> EventRepositoryInstance { get; private set; }

        public DatabaseFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            EventRepositoryInstance = new GenericMongoRepository<Event>(database, "Events");
        }
    }
}
