using AutoFixture;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Mapper;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;
using TicketingSystem.WebApi.Controllers;
using Xunit;

namespace TicketingSystem.IntegrationTests
{
    public class FixtureTestsBase
    {
        public FixtureTestsBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            fixture = new Fixture();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BusinessLogicMappingProfile());
            });
            mapper = mapperConfig.CreateMapper();

            _eventService = new EventService(_dbFixture.EventRepositoryInstance, mapper);
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

        public readonly string CartId = Guid.NewGuid().ToString();

        private readonly IMapper mapper;

        private readonly DatabaseFixture _dbFixture;
        protected readonly Fixture fixture;

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

        private readonly IEventService _eventService;
        private readonly IEventSectionService _eventSectionService;
        private readonly IPaymentService _paymentService;
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly IVenueService _venueService;
        private readonly ISectionService _sectionService;

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

            EventSectionsIds = await CreateEntities(_dbFixture.EventSectionRepositoryInstance, eventSections);
        }
        private static async Task<List<string>> CreateEntities<TEntity>(
            IMongoRepository<TEntity> repository, IEnumerable<TEntity> entities, CancellationToken ct = default)
            where TEntity : class, IHasId
        {
            foreach (var entity in entities)
            {
                await repository.CreateAsync(entity, ct);
            }

            return entities.Select(x => x.Id).ToList();
        }

        private static async Task RemoveEntities<TEntity>(
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