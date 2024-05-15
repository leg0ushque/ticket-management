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
    public class OrdersControllerTests
    {
        private const int ExistingCartItemsAmount = 5;

        private readonly string _cartId = Guid.NewGuid().ToString();
        private readonly string _seatId = Guid.NewGuid().ToString();

        private readonly IFixture _fixture;

        private readonly PaymentDto _payment;
        private readonly List<EventSectionDto> _eventSections;

        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IEventSectionService> _eventSectionServiceMock;

        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _fixture = new Fixture();

            _paymentServiceMock = new Mock<IPaymentService>();
            _eventSectionServiceMock = new Mock<IEventSectionService>();

            _payment = CreatePayment();
            _eventSections = CreateEventSections();

            SetupMocks();

            _controller = new OrdersController(_paymentServiceMock.Object, _eventSectionServiceMock.Object);
        }

        [Fact]
        public async Task GetCartItems_WhenInvoked_ShouldReturnOkWithResponse()
        {
            // Act
            var response = await _controller.GetCartItems(_fixture.Create<string>());

            // Assert
            _paymentServiceMock.Verify(s =>
                s.GetIncompletePayment(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            (responseObject.Value as CartItemDto[]).Should().BeEquivalentTo(_payment.CartItems);
        }

        [Fact]
        public async Task AddSeatToCart_WhenInvoked_ShouldReturnOkWithResponse()
        {
            // Arrange
            var requestBody = _fixture.Build<AddCartModel>()
                .With(m => m.EventId)
                .With(m => m.SeatId, _seatId)
                .With(m => m.Price)
                .With(m => m.PriceOption)
                .With(m => m.UserId)
                .Create();

            // Act
            var response = await _controller.AddSeatToCart(_cartId, requestBody);

            // Assert
            _eventSectionServiceMock.Verify(s =>
                s.GetSectionBySeatIdAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentServiceMock.Verify(s =>
                s.AppendCartItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CartItemDto>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            var responseObjectValue = responseObject.Value as PaymentStateModel;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            responseObjectValue.ItemsAmount.Should().Be(ExistingCartItemsAmount + 1);
            responseObjectValue.State.Should().Be(PaymentState.InProgress);
        }

        [Theory]
        [InlineData("","","")]
        [InlineData("eventId", "", "")]
        [InlineData("eventId", "seatId", "")]
        public async Task AddSeatToCart_WhenInvokedWithEmptyBodyFields_ShouldReturnBadRequest(
            string eventId, string seatId, string cartId)
        {
            // Arrange
            var requestBody = _fixture.Build<AddCartModel>()
                .With(m => m.EventId, eventId)
                .With(m => m.SeatId, seatId)
                .With(m => m.Price)
                .With(m => m.PriceOption)
                .With(m => m.UserId)
                .Create();

            // Act
            var response = await _controller.AddSeatToCart(cartId, requestBody);

            // Assert
            _paymentServiceMock.Verify(s =>
                s.AppendCartItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CartItemDto>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _eventSectionServiceMock.Verify(s =>
                s.GetSectionBySeatIdAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            var responseObject = response as BadRequestObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        private void SetupMocks()
        {
            _paymentServiceMock.Setup(s =>
                s.GetIncompletePayment(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_payment);

            _paymentServiceMock.Setup(s =>
                s.AppendCartItem(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CartItemDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentStateModel
                {
                    ItemsAmount = ExistingCartItemsAmount + 1,
                    State = PaymentState.InProgress
                });

            _eventSectionServiceMock.Setup(s =>
                s.GetSectionBySeatIdAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_eventSections.FirstOrDefault());
        }

        private PaymentDto CreatePayment()
        {
            return new PaymentDto()
            {
                CartId = _cartId,
                State = PaymentState.InProgress,
                CartItems = _fixture.Build<CartItemDto>()
                            .With(ci => ci.Id)
                            .With(ci => ci.EventId)
                            .With(ci => ci.EventSectionId)
                            .With(ci => ci.EventSectionClass)
                            .With(ci => ci.EventSectionNumber)
                            .With(ci => ci.EventSeatId)
                            .With(ci => ci.EventRowNumber)
                            .With(ci => ci.EventSeatNumber)
                            .With(ci => ci.Price)
                        .CreateMany(ExistingCartItemsAmount).ToArray()
            };
        }
        private List<EventSectionDto> CreateEventSections()
        {
            return _fixture.Build<EventSectionDto>()
                .With(x => x.Number)
                .With(x => x.Class)
                .With(x => x.EventId)
                .With(x => x.EventSeats,
                    _fixture.Build<EventSeatDto>()
                        .With(es => es.Id, _seatId)
                        .With(es => es.RowNumber)
                        .With(es => es.SeatNumber)
                        .With(es => es.CartId, _cartId)
                        .With(es => es.PaymentId)
                        .With(es => es.Price)
                        .With(es => es.State, EventSeatState.Available)
                    .CreateMany(10).ToArray())
                .CreateMany(4).ToList();
        }
    }
}
