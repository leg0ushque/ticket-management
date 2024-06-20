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
using TicketingSystem.WebApi.Controllers;

namespace TicketingSystem.IntegrationTests
{
    public class FixtureTestsBase
    {
        public readonly string CartId = Guid.NewGuid().ToString();

        protected readonly Fixture fixture;
        protected readonly DatabaseFixture _dbFixture;
        private readonly IMapper mapper;

        protected readonly IEventService _eventService;
        protected readonly IEventSectionService _eventSectionService;
        protected readonly IPaymentService _paymentService;
        protected readonly ITicketService _ticketService;
        protected readonly IUserService _userService;
        protected readonly IVenueService _venueService;
        protected readonly ISectionService _sectionService;

        public FixtureTestsBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            fixture = new Fixture();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });
            mapper = mapperConfig.CreateMapper();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheOptions = Options.Create<CacheOptions>(new CacheOptions());

            _eventService = new EventService(_dbFixture.EventRepositoryInstance, mapper, memoryCache, cacheOptions);
            _eventSectionService = new EventSectionService(_dbFixture.EventSectionRepositoryInstance, mapper);

            _paymentService = new PaymentService(_dbFixture.PaymentRepositoryInstance, mapper);
            _ticketService = new TicketService(_dbFixture.TicketRepositoryInstance, mapper);
            _userService = new UserService(_dbFixture.UserRepositoryInstance, mapper);
            _venueService = new VenueService(_dbFixture.VenueRepositoryInstance,
                _dbFixture.SectionRepositoryInstance, mapper);
            _sectionService = new SectionService(_dbFixture.SectionRepositoryInstance, mapper);

            EventsController = new EventsController(_eventService, _eventSectionService);
            PaymentsController = new PaymentsController(_paymentService, _eventSectionService);
            OrdersController = new OrdersController(_paymentService, _eventSectionService);
        }

        public EventsController EventsController { get; set; }
        public PaymentsController PaymentsController { get; set; }
        public OrdersController OrdersController { get; set; }

        public List<string> EventsIds { get; set; }
        public List<string> EventSectionsIds { get; set; }
        public List<string> PaymentsIds { get; set; }
        public List<string> TicketsIds { get; set; }
        public List<string> UsersIds { get; set; }
        public List<string> VenuesIds { get; set; }
        public List<string> SectionsIds { get; set; }

        public Task DeleteGeneratedEntities(CancellationToken ct = default)
        {
            return RemoveEntities(_dbFixture.EventSectionRepositoryInstance, EventSectionsIds, ct)
                .ContinueWith(x => RemoveEntities(_dbFixture.EventRepositoryInstance, EventsIds), ct);
        }

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
    }
}