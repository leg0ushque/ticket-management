using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.IntegrationTests
{
    public class DatabaseFixture
    {
        public IMongoRepository<Event> EventRepositoryInstance { get; private set; }
        public IMongoRepository<EventSection> EventSectionRepositoryInstance { get; private set; }
        public IMongoRepository<Payment> PaymentRepositoryInstance { get; private set; }
        public IMongoRepository<Ticket> TicketRepositoryInstance { get; private set; }
        public IMongoRepository<User> UserRepositoryInstance { get; private set; }
        public IMongoRepository<Venue> VenueRepositoryInstance { get; private set; }
        public IMongoRepository<Section> SectionRepositoryInstance { get; private set; }

        public DatabaseFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            var mongoDbFactory = new MongoDbFactory(connectionString, databaseName);

            EventRepositoryInstance = new EventRepository(mongoDbFactory);
            EventSectionRepositoryInstance = new EventSectionRepository(mongoDbFactory);
            PaymentRepositoryInstance = new PaymentRepository(mongoDbFactory);
            TicketRepositoryInstance = new TicketRepository(mongoDbFactory);
            UserRepositoryInstance = new UserRepository(mongoDbFactory);
            VenueRepositoryInstance = new VenueRepository(mongoDbFactory);
            SectionRepositoryInstance = new SectionRepository(mongoDbFactory);
        }
    }
}
