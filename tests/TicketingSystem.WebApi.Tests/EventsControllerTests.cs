using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.ObjectModel;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.WebApi.Controllers;
using Xunit;

namespace TicketingSystem.WebApi.Tests
{
    public class EventsControllerTests
    {
        private readonly IFixture _fixture;

        private readonly List<EventDto> _events;
        private readonly List<EventSectionDto> _eventSections;

        private readonly Mock<IEventService> _eventServiceMock;
        private readonly Mock<IEventSectionService> _eventSectionServiceMock;

        private readonly EventsController _controller;

        public EventsControllerTests()
        {
            _fixture = new Fixture();

            _eventServiceMock = new Mock<IEventService>();
            _eventSectionServiceMock = new Mock<IEventSectionService>();

            _events = _fixture
                .Build<EventDto>()
                .With(x => x.Id, Guid.NewGuid().ToString())
                .CreateMany(5)
                .ToList();

            _eventSections = CreateEventSections();

            SetupMocks();

            _controller = new EventsController(_eventServiceMock.Object, _eventSectionServiceMock.Object);
        }

        [Fact]
        public async Task GetEvents_WhenInvoked_ShouldReturnOkWithResponse()
        {
            // Act
            var response = await _controller.GetEvents();

            // Assert
            _eventServiceMock.Verify(s =>
                s.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            (responseObject.Value as ReadOnlyCollection<EventDto>).Should().BeEquivalentTo(_events);
        }

        [Fact]
        public async Task GetFullSeats_WhenInvoked_ShouldReturnOkWithResponse()
        {
            var eventId = _events.FirstOrDefault().Id;
            var sectionId = _eventSections.FirstOrDefault().Id;

            // Act
            var response = await _controller.GetFullSeats(eventId, sectionId);

            // Assert
            _eventServiceMock.Verify(s =>
                s.GetByIdAsync(
                        It.Is<string>(eId => eId == eventId),
                        It.IsAny<CancellationToken>()),
                Times.Once);

            _eventSectionServiceMock.Verify(s =>
                    s.GetSeatsInfo(
                        It.Is<string>(sId => sId == sectionId),
                        It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            var responseObjectValue = responseObject.Value as List<EventSeatInfoModel>;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            responseObjectValue.Should().BeEquivalentTo(GetEventSeatInfo(_eventSections.FirstOrDefault()));
        }

        private void SetupMocks()
        {
            _eventServiceMock.Setup(s =>
                s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_events.AsReadOnly());

            _eventServiceMock.Setup(s =>
                s.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_events.FirstOrDefault());

            _eventSectionServiceMock.Setup(s =>
                s.GetSeatsInfo(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetEventSeatInfo(_eventSections.FirstOrDefault()));
        }

        private List<EventSectionDto> CreateEventSections()
        {
            return _fixture.Build<EventSectionDto>()
                .With(x => x.Number)
                .With(x => x.Class)
                .With(x => x.EventId, _events.FirstOrDefault().Id)
                .With(x => x.EventSeats,
                    _fixture.Build<EventSeatDto>()
                        .With(es => es.RowNumber)
                        .With(es => es.SeatNumber)
                        .With(es => es.PaymentId)
                        .With(es => es.Price)
                        .With(es => es.State, EventSeatState.Available)
                    .CreateMany(10).ToArray())
                .CreateMany(4).ToList();
        }

        private List<EventSeatInfoModel> GetEventSeatInfo(EventSectionDto eventSection)
        {
            return eventSection.EventSeats.Select(es => new EventSeatInfoModel
            {
                EventSectionId = eventSection.Id,
                EventSectionNumber = eventSection.Number,
                EventSectionClass = eventSection.Class,
                RowNumber = es.RowNumber,
                SeatNumber = es.SeatNumber,
                Price = es.Price,
                EventSeatState = es.State,
            }).ToList();
        }
    }
}
