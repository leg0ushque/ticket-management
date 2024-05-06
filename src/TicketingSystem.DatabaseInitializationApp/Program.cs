using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Factories;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DatabaseInitializationApp
{
    public static class Program
    {
        static async Task Main()
        {
            var env = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? throw new ArgumentException("ENV VAR");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"settings.{env}.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            await ResetDatabase(connectionString, databaseName);
        }

        static async Task ResetDatabase(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);

            var databases = client.ListDatabases().ToList();
            if (databases.Any(database => database["name"] == databaseName))
            {
                Console.WriteLine("Database exists. Press ENTER to drop & recreate it");
                Console.ReadLine();

                client.DropDatabase(databaseName);
            }
            else
            {
                Console.WriteLine("Database does not exist.");
            }

            var factory = new MongoDbFactory(connectionString, databaseName);

            IMongoRepository<Event> eventRepository = new EventRepository(factory);
            IMongoRepository<EventSection> eventSectionRepository = new EventSectionRepository(factory);
            IMongoRepository<Section> sectionRepository = new SectionRepository(factory);
            IMongoRepository<Ticket> ticketRepository = new TicketRepository(factory);
            IMongoRepository<User> userRepository = new UserRepository(factory);
            IMongoRepository<Venue> venueRepository = new VenueRepository(factory);
            IMongoRepository<Payment> paymentRepository = new PaymentRepository(factory);

            await InitializeDatabase(
                eventRepository,
                eventSectionRepository,
                sectionRepository,
                ticketRepository,
                userRepository,
                venueRepository,
                paymentRepository);
        }

        private static async Task InitializeDatabase(
            IMongoRepository<Event> eventRepository,
            IMongoRepository<EventSection> eventSectionRepository,
            IMongoRepository<Section> sectionRepository,
            IMongoRepository<Ticket> ticketRepository,
            IMongoRepository<User> userRepository,
            IMongoRepository<Venue> venueRepository,
            IMongoRepository<Payment> paymentRepository)
        {
            var dtNow = DateTime.Now;

            var cartId = Guid.NewGuid().ToString();
            Console.WriteLine($"CartId: {cartId}");

            // USERS

            var users = new List<User>
            {
                new() { Email = "ipetrov@tickets.by", FirstName = "Ivan", LastName = "Petrov", Role = UserRole.User, CartId = cartId },
                new() { Email = "adm1@tickets.by", FirstName = "Alex", LastName = "Adminov", Role = UserRole.Admin, CartId = Guid.NewGuid().ToString() }
            };

            await CreateEntities(userRepository, users);

            // VENUES

            var venues = new List<Venue>
            {
                new() {
                    Address = "Wimbledon, England",
                    Description = "A great stadium",
                    Name = "Wimbledon Stadium",
                    Phone = "123-456-789"
                }
            };
            await CreateEntities(venueRepository, venues);

            // SECTIONS & ROWS

            var sections = new List<Section>
            {
                new() {
                    Class = "A", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 5).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                    ]
                },
                new() {
                    Class = "A", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 5).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                    ]
                },
                new() {
                    Class = "B", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 20).ToArray() },
                    ]
                },
                new() {
                    Class = "B", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = Enumerable.Range(1, 10).ToArray() },
                        new Row { Number = 2, SeatNumbers = Enumerable.Range(1, 20).ToArray() },
                    ]
                }
            };

            await CreateEntities(sectionRepository, sections);

            // EVENTS

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
            Console.WriteLine($"EventId: {events[0].Id}");

            // EVENT SECTIONS & EVENT SEATS

            var firstSection = new EventSection
            {
                Number = sections[0].Number,
                Class = sections[0].Class,
                EventId = events[0].Id,
                EventSeats =
                (
                    from row in sections[0].Rows
                    from seatNumber in row.SeatNumbers
                    select new EventSeat
                    {
                        RowNumber = row.Number,
                        SeatNumber = seatNumber,
                        CartId = cartId,
                        PaymentId = null,
                        Price = 5.5m,
                        State = EventSeatState.Available,
                    }
                ).ToArray()
            };

            var secondSection = new EventSection
            {
                Number = sections[1].Number,
                Class = sections[1].Class,
                EventId = events[0].Id,
                EventSeats =
                (
                    from row in sections[1].Rows
                    from seatNumber in row.SeatNumbers
                    select new EventSeat
                    {
                        RowNumber = row.Number,
                        SeatNumber = seatNumber,
                        CartId = cartId,
                        PaymentId = null,
                        Price = 5.5m,
                        State = EventSeatState.Available,
                    }
                ).ToArray()
            };

            firstSection.EventSeats[0].State = EventSeatState.Booked;
            firstSection.EventSeats[1].State = EventSeatState.Booked;
            secondSection.EventSeats[0].State = EventSeatState.Sold;

            var eventSections = new List<EventSection> { firstSection, secondSection };

            await CreateEntities(eventSectionRepository, eventSections);

            // PAYMENTS & CART ITEMS

            var payments = new List<Payment>
            {
                new() { CartId = cartId, State = PaymentState.InProgress, CartItems =
                [
                    new CartItem
                    {
                        EventId = events[0].Id,
                        EventSectionId = firstSection.Id,
                        EventSectionClass = firstSection.Class,
                        EventSectionNumber = firstSection.Number,
                        EventSeatId = firstSection.EventSeats[0].Id,
                        EventRowNumber = firstSection.EventSeats[0].RowNumber,
                        EventSeatNumber = firstSection.EventSeats[0].SeatNumber,
                        Price = firstSection.EventSeats[0].Price
                    },
                    new CartItem
                    {
                        EventId = events[0].Id,
                        EventSectionId = firstSection.Id,
                        EventSeatId = firstSection.EventSeats[1].Id,
                        EventSectionClass = firstSection.Class,
                        EventSectionNumber = firstSection.Number,
                        EventRowNumber = firstSection.EventSeats[1].RowNumber,
                        EventSeatNumber = firstSection.EventSeats[1].SeatNumber,
                        Price = firstSection.EventSeats[1].Price
                    },
                ] },
                new() { CartId = cartId, State = PaymentState.Completed, CartItems =
                [
                    new CartItem
                    {
                        EventId = events[0].Id,
                        EventSectionId = secondSection.Id,
                        EventSeatId = secondSection.EventSeats[0].Id,
                        EventRowNumber = secondSection.EventSeats[0].RowNumber,
                        EventSeatNumber = secondSection.EventSeats[0].SeatNumber,
                        EventSectionClass = secondSection.Class,
                        EventSectionNumber = secondSection.Number,
                        Price = secondSection.EventSeats[0].Price
                    },
                ]
                },
            };

            await CreateEntities(paymentRepository, payments);

            var tickets = new List<Ticket>
            {
                new() {
                    EventId = events[0].Id,
                    EventSeatId = eventSections[1].EventSeats[0].Id,
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
