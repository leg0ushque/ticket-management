using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Enums;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DatabaseInitializationApp
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? throw new ArgumentException("ENV VAR");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.{env}.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            var factory = new MongoDbFactory(connectionString, databaseName);

            IMongoRepository<CartItem> cartItemRepository = new CartItemRepository(factory);
            IMongoRepository<Event> eventRepository = new EventRepository(factory);
            IMongoRepository<EventSeat> eventSeatRepository = new EventSeatRepository(factory);
            IMongoRepository<EventSection> eventSectionRepository = new EventSectionRepository(factory);
            IMongoRepository<Section> sectionRepository = new SectionRepository(factory);
            IMongoRepository<Ticket> ticketRepository = new TicketRepository(factory);
            IMongoRepository<User> userRepository = new UserRepository(factory);
            IMongoRepository<Venue> venueRepository = new VenueRepository(factory);

            await InitializeDatabase(
                cartItemRepository,
                eventRepository,
                eventSeatRepository,
                eventSectionRepository,
                sectionRepository,
                ticketRepository,
                userRepository,
                venueRepository);
        }

        private static async Task InitializeDatabase(
            IMongoRepository<CartItem> cartItemRepository,
            IMongoRepository<Event> eventRepository,
            IMongoRepository<EventSeat> eventSeatRepository,
            IMongoRepository<EventSection> eventSectionRepository,
            IMongoRepository<Section> sectionRepository,
            IMongoRepository<Ticket> ticketRepository,
            IMongoRepository<User> userRepository,
            IMongoRepository<Venue> venueRepository)
        {
            var dtNow = DateTime.Now;

            var venues = new List<Venue>
            {
                new() {
                    Id = Guid.NewGuid().ToString(),
                    Address = "Wimbledon, England",
                    Description = "A great stadium",
                    Name = "Wimbledon Stadium",
                    Phone = "123-456-789"
                }
            };
            await CreateEntities(venueRepository, venues);

            var users = new List<User>
            {
                new() { Id = Guid.NewGuid().ToString(), Email = "ipetrov@tickets.by", FirstName = "Ivan", LastName = "Petrov", Role = UserRole.User },
                new() { Id = Guid.NewGuid().ToString(), Email = "adm1@tickets.by", FirstName = "Alex", LastName = "Adminov", Role = UserRole.Admin }
            };

            await CreateEntities(userRepository, users);

            var sections = new List<Section>
            {
                new() {
                    Id = Guid.NewGuid().ToString(),
                    Class = "A", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 5).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                    ]
                },
                new() {
                    Id = Guid.NewGuid().ToString(),
                    Class = "A", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 5).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                    ]
                },
                new() {
                    Id = Guid.NewGuid().ToString(),
                    Class = "B", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 20).ToArray() },
                    ]
                },
                new() {
                    Id = Guid.NewGuid().ToString(),
                    Class = "B", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 20).ToArray() },
                    ]
                }
            };

            await CreateEntities(sectionRepository, sections);

            var events = new List<Event>
            {
                new() {
                    Name = "Amazing Match 2024",
                    Description = "You have never seen anything more marvelous than this!",
                    StartTime = dtNow.AddDays(3),
                    EndTime= dtNow.AddDays(3).AddHours(4),
                    State = EventState.Available,
                    VenueId = venues[0].Id,
                }
            };

            await CreateEntities(eventRepository, events);

            var firstSection = sections[0];
            var firstRow = firstSection.Rows[0];
            var firstEventSeats = firstRow.SeatNumbers
                .Select(seatNumber => new EventSeat
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = seatNumber,
                    State = EventSeatState.Available
                })
                .ToArray();

            var secondSection = sections[2];
            var secondRow = secondSection.Rows[1];
            var secondEventSeats = secondRow.SeatNumbers
                .Select(seatNumber => new EventSeat
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = seatNumber,
                    State = EventSeatState.Available
                })
                .ToArray();

            var eventSeats = new List<EventSeat>(firstEventSeats);

            eventSeats[0].State = EventSeatState.Booked;
            eventSeats[1].State = EventSeatState.Booked;
            eventSeats[2].State = EventSeatState.Sold;

            eventSeats.AddRange(secondEventSeats);

            await CreateEntities(eventSeatRepository, eventSeats);

            var eventSections = new List<EventSection>
            {
                new() {
                    Class = firstSection.Class, Number = firstSection.Number,
                    EventId = events[0].Id,
                    EventRows = [
                        new EventRow
                        {
                            Number = firstRow.Number,
                            Price = 12m,
                            EventSeatsIds = firstEventSeats.Select(es => es.Id).ToArray()
                        },
                    ]
                },
                new() {
                    Class = secondSection.Class, Number = secondSection.Number,
                    EventId = events[0].Id,
                    EventRows = [
                        new EventRow
                        {
                            Number = secondRow.Number,
                            Price = 7.5m,
                            EventSeatsIds = secondEventSeats.Select(es => es.Id).ToArray()
                        },
                    ]
                },
            };

            await CreateEntities(eventSectionRepository, eventSections);

            var cartId = Guid.NewGuid().ToString();

            var cartItems = new List<CartItem>
            {
                new() { CartId = cartId, CreatedOn = dtNow.AddDays(-1), EventSeatId = eventSeats[0].Id },
                new() { CartId = cartId, CreatedOn = dtNow.AddDays(-1), EventSeatId = eventSeats[1].Id }
            };

            await CreateEntities(cartItemRepository, cartItems);

            var tickets = new List<Ticket>
            {
                new() {
                    EventId = events[0].Id,
                    EventSeatId = eventSeats[2].Id,
                    Price = 6m,
                    PriceOption = PriceOption.Child,
                    PurchasedOn = dtNow.AddDays(-2),
                    State = TicketState.Purchased,
                    UserId = users[0].Id
                }
            };

            await CreateEntities(ticketRepository, tickets);

            Console.WriteLine("Done. Press ENTER...");
            Console.ReadLine();
        }

        private static async Task CreateEntities<TEntity>(IRepository<TEntity> repository, IList<TEntity> entities)
            where TEntity : IHasId
        {
            foreach (var entity in entities)
            {
                await repository.CreateAsync(entity);
            }

            Console.WriteLine($"{typeof(TEntity)}s created...");
        }
    }
}
