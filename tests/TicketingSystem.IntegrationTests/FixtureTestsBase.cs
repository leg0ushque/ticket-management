using AutoFixture;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Mapper;
using TicketingSystem.BusinessLogic.Options;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using TicketingSystem.Messaging;
using TicketingSystem.Messaging.Options;
using TicketingSystem.Messaging.Producer;
using TicketingSystem.WebApi.Controllers;

namespace TicketingSystem.IntegrationTests
{
    public class FixtureTestsBase : IDisposable
    {
        public readonly string CartId = Guid.NewGuid().ToString();

        protected readonly Fixture fixture;
        protected readonly DatabaseFixture _dbFixture;
        protected readonly IMapper _mapper;

        protected readonly IEventService _eventService;
        protected readonly IEventSectionService _eventSectionService;
        protected readonly IPaymentService _paymentService;
        protected readonly ITicketService _ticketService;
        protected readonly IUserService _userService;
        protected readonly IVenueService _venueService;
        protected readonly ISectionService _sectionService;
        protected readonly INotificationService _notificationService;

        public FixtureTestsBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            fixture = new Fixture();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheOptions = Options.Create<CacheOptions>(new CacheOptions());

            _eventService = new EventService(_dbFixture.EventRepositoryInstance, _mapper, memoryCache, cacheOptions);
            _eventSectionService = new EventSectionService(_dbFixture.EventSectionRepositoryInstance, _mapper);

            _paymentService = new PaymentService(_dbFixture.PaymentRepositoryInstance, _mapper);
            _ticketService = new TicketService(_dbFixture.TicketRepositoryInstance, _mapper);
            _userService = new UserService(_dbFixture.UserRepositoryInstance, _mapper);
            _venueService = new VenueService(_dbFixture.VenueRepositoryInstance,
                _dbFixture.SectionRepositoryInstance, _mapper);
            _sectionService = new SectionService(_dbFixture.SectionRepositoryInstance, _mapper);
            _notificationService = new NotificationService(_dbFixture.NotificationRepositoryInstance, _mapper);

            var kafkaOptions = Options.Create<KafkaOptions>(
                new KafkaOptions
                {
                    BootstrapServer = "localhost:9092",
                    ClientId = "processing",
                    Topic = "ticketing-emails"
                });

            var kafkaConfigProvider = new KafkaConfigurationProvider(kafkaOptions);
            var producerProvider = new KafkaProducerProvider(kafkaConfigProvider);
            IKafkaProducer kafkaProducer = new KafkaProducer(producerProvider, kafkaOptions, null);

            var kafkaNotificationService = new KafkaNotificationService(kafkaProducer, _notificationService, _eventService);

            EventsController = new EventsController(_eventService, _eventSectionService);
            PaymentsController = new PaymentsController(kafkaNotificationService, _paymentService, _eventSectionService);
            OrdersController = new OrdersController(kafkaNotificationService, _paymentService, _eventSectionService);
        }

        public EventsController EventsController { get; set; }
        public PaymentsController PaymentsController { get; set; }
        public OrdersController OrdersController { get; set; }

        public List<string> EventsIds { get; set; } = new();
        public List<string> EventSectionsIds { get; set; } = new();
        public List<string> PaymentsIds { get; set; } = new();
        public List<string> TicketsIds { get; set; } = new();
        public List<string> UsersIds { get; set; } = new();
        public List<string> VenuesIds { get; set; } = new();
        public List<string> SectionsIds { get; set; } = new();

        //public Task DeleteGeneratedEntities(CancellationToken ct = default)
        //{
        //    return RemoveEntities(_dbFixture.EventSectionRepositoryInstance, EventSectionsIds, ct)
        //        .ContinueWith(x => RemoveEntities(_dbFixture.EventRepositoryInstance, EventsIds), ct);
        //}

        public async Task GenerateEntities(CancellationToken ct = default)
        {
            // EVENTS

            var eventEntity = fixture
                .Build<Event>()
                .With(x => x.Id, Guid.NewGuid().ToString())
                .With(x => x.Description)
                .Create();

            EventsIds = await CreateEntities(_dbFixture.EventRepositoryInstance, [eventEntity], ct);

            // EVENT SECTIONS

            var eventSections = fixture.Build<EventSection>()
                .With(x => x.Number)
                .With(x => x.Class)
                .With(x => x.EventId, eventEntity.Id)
                .With(x => x.EventSeats,
                    fixture.Build<EventSeat>()
                        .With(es => es.RowNumber)
                        .With(es => es.SeatNumber)
                        .With(es => es.PaymentId)
                        .With(es => es.Price)
                        .With(es => es.State, EventSeatState.Available)
                    .CreateMany(7).ToArray())
                .CreateMany(3).ToList();

            EventSectionsIds = await CreateEntities(_dbFixture.EventSectionRepositoryInstance, eventSections, ct);
        }

        protected static async Task<List<string>> CreateEntities<TEntity>(
            IMongoRepository<TEntity> repository, IEnumerable<TEntity> entities, CancellationToken ct = default)
            where TEntity : class, IHasId
        {
            foreach (var entity in entities)
            {
                await repository.CreateAsync(entity, ct);
            }

            return entities.Select(x => x.Id).ToList();
        }

        protected static async Task RemoveEntities<TEntity>(
            IMongoRepository<TEntity> repository, IEnumerable<string> entitiesIds, CancellationToken ct = default)
            where TEntity : class, IHasId
        {
            foreach (var id in entitiesIds)
            {
                await repository.DeleteAsync(id, ct);
            }
        }

        public void Dispose()
        {
            RemoveEntities(_dbFixture.EventSectionRepositoryInstance, EventSectionsIds).GetAwaiter().GetResult();
            RemoveEntities(_dbFixture.EventRepositoryInstance, EventsIds).GetAwaiter().GetResult();
            RemoveEntities(_dbFixture.PaymentRepositoryInstance, PaymentsIds).GetAwaiter().GetResult();
        }
    }
}