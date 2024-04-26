using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Enums;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.DatabaseInitializationApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();

            var connectionString = config.GetConnectionString("connectionString");
            var databaseName = config.GetSection("databaseName").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            IMongoRepository<CartItem> cartItemRepository = new GenericMongoRepository<CartItem>(database, "CartItems");
            IMongoRepository<Event> eventRepository = new GenericMongoRepository<Event>(database, "Events");
            IMongoRepository<EventSeat> eventSeatRepository = new GenericMongoRepository<EventSeat>(database, "EventSeats");
            IMongoRepository<EventSection> eventSectionRepository = new GenericMongoRepository<EventSection>(database, "EventSections");
            IMongoRepository<PriceOption> priceOptionRepository = new GenericMongoRepository<PriceOption>(database, "PriceOptions");
            IMongoRepository<Section> sectionRepository = new GenericMongoRepository<Section>(database, "Sections");
            IMongoRepository<Ticket> ticketRepository = new GenericMongoRepository<Ticket>(database, "Tickets");
            IMongoRepository<User> userRepository = new GenericMongoRepository<User>(database, "Users");
            IMongoRepository<Venue> venueRepository = new GenericMongoRepository<Venue>(database, "Venues");

            await InitializeDatabase(
                cartItemRepository,
                eventRepository,
                eventSeatRepository,
                eventSectionRepository,
                priceOptionRepository,
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
            IMongoRepository<PriceOption> priceOptionRepository,
            IMongoRepository<Section> sectionRepository,
            IMongoRepository<Ticket> ticketRepository,
            IMongoRepository<User> userRepository,
            IMongoRepository<Venue> venueRepository)
        {
            var dtNow = DateTime.Now;

            var venues = new List<Venue>
            {
                new Venue {
                    Id = Guid.NewGuid().ToString(),
                    Address = "Wimbledon, England",
                    Description = "A great stadium",
                    Name = "Wimbledon Stadium",
                    Phone = "123-456-789"
                }
            };

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), Email = "ipetrov@tickets.by", FirstName = "Ivan", LastName = "Petrov", Role = UserRole.User },
                new User { Id = Guid.NewGuid().ToString(), Email = "adm1@tickets.by", FirstName = "Alex", LastName = "Adminov", Role = UserRole.Admin }
            };

            var priceOptions = new List<PriceOption>
            {
                new PriceOption { Id = Guid.NewGuid().ToString(), Name = "Child", Coefficient = 0.5m },
                new PriceOption { Id = Guid.NewGuid().ToString(), Name = "Adult", Coefficient = 1 },
                new PriceOption { Id = Guid.NewGuid().ToString(), Name = "VIP", Coefficient = 1.5m },
            };

            var sections = new List<Section>
            {
                new Section {
                    Id = Guid.NewGuid().ToString(),
                    Class = "A", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = [ 1, 2, 3, 4, 5 ] },
                        new Row { Number = 2, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ] },
                    ]
                },
                new Section {
                    Id = Guid.NewGuid().ToString(),
                    Class = "A", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = [ 1, 2, 3, 4, 5 ] },
                        new Row { Number = 2, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ] },
                    ]
                },
                new Section {
                    Id = Guid.NewGuid().ToString(),
                    Class = "B", Number = 1,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ] },
                        new Row { Number = 2, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,] },
                    ]
                },
                new Section {
                    Id = Guid.NewGuid().ToString(),
                    Class = "B", Number = 2,
                    VenueId = venues[0].Id,
                    Rows = [
                        new Row { Number = 1, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ] },
                        new Row { Number = 2, SeatNumbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,] },
                    ]
                }
            };

            var events = new List<Event>
            {
                new Event {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Amazing Match 2024",
                    Description = "You have never seen anything more marvelous than this!",
                    StartTime = dtNow.AddDays(3),
                    EndTime= dtNow.AddDays(3).AddHours(4),
                    State = EventState.Available,
                    VenueId = venues[0].Id,
                }
            };

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
            eventSeats.AddRange(secondEventSeats);

            var eventSections = new List<EventSection>
            {
                new EventSection {
                    Id = Guid.NewGuid().ToString(),
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
                new EventSection {
                    Id = Guid.NewGuid().ToString(),
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

            var cartId = Guid.NewGuid().ToString();

            eventSeats[0].State = EventSeatState.Booked;
            eventSeats[1].State = EventSeatState.Booked;

            var cartItems = new List<CartItem>
            {
                new CartItem { Id = Guid.NewGuid().ToString(), CartId = cartId, CreatedOn = dtNow.AddDays(-1), EventSeatId = eventSeats[0].Id },
                new CartItem { Id = Guid.NewGuid().ToString(), CartId = cartId, CreatedOn = dtNow.AddDays(-1), EventSeatId = eventSeats[1].Id }
            };

            eventSeats[2].State = EventSeatState.Sold;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid().ToString(),
                    EventId = events[0].Id,
                    EventSeatId = eventSeats[2].Id,
                    Price = 6m,                         // Base 12 * 0.5 (child)
                    PriceOptionId = priceOptions[0].Id, // Child,
                    PurchasedOn = dtNow.AddDays(-2),
                    State = TicketState.Purchased,
                    UserId = users[0].Id
                }
            };

            await CreateEntities(eventRepository, events);

            await CreateEntities(eventSeatRepository, eventSeats);

            await CreateEntities(cartItemRepository, cartItems);

            await CreateEntities(eventSectionRepository, eventSections);

            await CreateEntities(priceOptionRepository, priceOptions);

            await CreateEntities(sectionRepository, sections);

            await CreateEntities(ticketRepository, tickets);

            await CreateEntities(userRepository, users);

            await CreateEntities(venueRepository, venues);
        }

        private static async Task CreateEntities<TEntity>(IRepository<TEntity, string> repository, IList<TEntity> entities)
            where TEntity : IStringKeyEntity
        {
            foreach (var entity in entities)
            {
                await repository.CreateAsync(entity);
            }
        }
    }
}
