using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
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
            var env = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? throw new ArgumentException("ENV VAR");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.{env}.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            EventRepositoryInstance = new GenericMongoRepository<Event>(database, "Events");
        }
    }
}
