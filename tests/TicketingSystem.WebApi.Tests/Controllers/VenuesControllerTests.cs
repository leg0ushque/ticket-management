using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.WebApi.Controllers;
using TicketingSystem.WebApi.Models;
using Xunit;


namespace TicketingSystem.WebApi.Tests.Controllers
{
    public class VenuesControllerTests
    {
        private const int ExistingEntitiesAmount = 5;

        private readonly IFixture _fixture;

        private readonly List<VenueDto> _venues;
        private readonly List<SectionDto> _venueSections;

        private readonly Mock<IVenueService> _venueServiceMock;

        private readonly VenuesController _controller;

        public VenuesControllerTests()
        {
            _fixture = new Fixture();

            _venueServiceMock = new Mock<IVenueService>();

            _venues = _fixture
                .Build<VenueDto>()
                .With(x => x.Id, Guid.NewGuid().ToString())
                .CreateMany(ExistingEntitiesAmount)
                .ToList();

            _venueSections = _fixture
                .Build<SectionDto>()
                .With(x => x.Id, Guid.NewGuid().ToString())
                .CreateMany(ExistingEntitiesAmount)
                .ToList();

            SetupMocks();

            _controller = new VenuesController(_venueServiceMock.Object);
        }

        private void SetupMocks()
        {
            _venueServiceMock
                .Setup(s => s.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_venues);

            _venueServiceMock
                .Setup(s => s.GetVenueSectionsAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_venueSections);
        }

        [Fact]
        public async Task GetVenues_WhenInvoked_ShouldReturnOkWithResponse()
        {
            // Act
            var response = await _controller.GetVenues();

            // Assert
            _venueServiceMock.Verify(s =>
                s.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            (responseObject.Value as IList<VenueDto>).Should().BeEquivalentTo(_venues);
        }

        [Fact]
        public async Task GetVenueSections_WhenGivenVenueId_ShouldReturnOkWithResponse()
        {
            // Arrange
            var venueId = "1";

            // Act
            var response = await _controller.GetVenueSections(venueId);

            // Assert
            _venueServiceMock.Verify(s =>
                s.GetVenueSectionsAsync(venueId, It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            (responseObject.Value as IList<SectionDto>).Should().BeEquivalentTo(_venueSections);
        }
    }
}
