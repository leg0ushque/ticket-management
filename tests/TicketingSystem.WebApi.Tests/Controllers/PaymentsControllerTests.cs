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
using Xunit;

namespace TicketingSystem.WebApi.Tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Fixture _fixture;

        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly Mock<IEventSectionService> _eventSectionServiceMock;
        private readonly PaymentsController _controller;
        private readonly PaymentDto _payment;
        private readonly List<EventSectionSeatsModel> _eventSections;

        public PaymentsControllerTests()
        {
            _fixture = new Fixture();

            _paymentServiceMock = new Mock<IPaymentService>();
            _eventSectionServiceMock = new Mock<IEventSectionService>();

            _controller = new PaymentsController(_paymentServiceMock.Object, _eventSectionServiceMock.Object);

            _payment = _fixture.Build<PaymentDto>()
                .With(p => p.Id)
                .With(p => p.State, PaymentState.InProgress)
                .Create();

            _eventSections = _fixture.Build<EventSectionSeatsModel>()
                .With(e => e.EventId)
                .With(e => e.SectionSeats, _fixture.Build<SectionSeatsModel>()
                    .With(s => s.SectionId)
                    .With(s => s.SeatIds, _fixture.CreateMany<string>(5).ToArray())
                    .CreateMany(5).ToArray())
                .CreateMany(5).ToList();

            SetupMocks();
        }

        [Fact]
        public async Task GetPaymentStatus_WhenGivenPaymentId_ShouldReturnOkWithResponse()
        {
            // Act
            var response = await _controller.GetPaymentStatus(_payment.Id);

            // Assert
            _paymentServiceMock.Verify(s => s.GetByIdAsync(It.Is<string>(s => s == _payment.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            var responseObject = response as OkObjectResult;
            responseObject.StatusCode.Should().Be(StatusCodes.Status200OK);
            responseObject.Value.Should().Be(_payment.State);
        }

        [Fact]
        public async Task CompletePayment_WhenGivenPaymentId_ShouldReturnOk()
        {
            // Act
            var response = await _controller.CompletePayment(_payment.Id);

            // Assert
            _paymentServiceMock.Verify(s => s.GetByIdAsync(It.Is<string>(s => s == _payment.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentServiceMock.Verify(s =>
                s.UpdatePaymentState(It.Is<string>(s => s == _payment.Id),
                    PaymentState.Completed,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            var statusResult = response as OkResult;
            statusResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task FailPayment_WhenGivenPaymentId_ShouldReturnOk()
        {
            // Arrange

            // Act
            var response = await _controller.FailPayment(_payment.Id);

            // Assert
            _paymentServiceMock.Verify(s => s.GetByIdAsync(It.Is<string>(s => s == _payment.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _paymentServiceMock.Verify(s => s.UpdatePaymentState(It.Is<string>(s => s == _payment.Id),
                    PaymentState.Failed,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            var statusResult = response as OkResult;
            statusResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        private void SetupMocks()
        {
            _paymentServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_payment);

            _paymentServiceMock.Setup(s => s.GetPaymentEventSeats(It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_eventSections);

            _paymentServiceMock.Setup(s => s.UpdatePaymentState(It.IsAny<string>(), It.IsAny<PaymentState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _eventSectionServiceMock.Setup(s => s.UpdateEventSeatsState(It.IsAny<string>(),
                    It.IsAny<SectionSeatsModel[]>(),
                    It.IsAny<EventSeatState>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }
    }
}
