using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.WebApi.Controllers;
using TicketingSystem.WebApi.Models;
using Xunit;

namespace TicketingSystem.IntegrationTests.Processes
{
    public class PaymentProcessTests(DatabaseFixture fixture) : FixtureTestsBase(fixture), IClassFixture<DatabaseFixture>
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GivenPaymentProcess_WhenPaymentSucceeded_ShouldUpdateSeatStateToSoldAndUpdatePaymentAndDeleteCartItems(bool paymentShouldBeSuccessful)
        {
            await Setup();

            // Acts & Assertions

            // Get all events

            var eventsResponseResponse = await EventsController.GetEvents() as OkObjectResult;
            var events = eventsResponseResponse.Value as IReadOnlyCollection<EventDto>;
            var mainEvent = events.FirstOrDefault();

            // Get all events sections of the event

            var eventSections = await GetEventSections(mainEvent.Id);
            var mainSection = eventSections.FirstOrDefault();

            var freeSeats = mainSection.EventSeats.Where(s => s.State == EventSeatState.Available).ToList();
            var firstSeat = freeSeats[0];
            var secondSeat = freeSeats[1];

            // Add one seat to cart

            var addCartRequestBody = new AddCartModel
            {
                EventId = mainEvent.Id,
                SeatId = firstSeat.Id,
                Price = firstSeat.Price,
                PriceOption = PriceOption.Adult,
            };

            var firstSeatAddResponse = await OrdersController.AddSeatToCart(CartId, addCartRequestBody) as OkObjectResult;
            var firstSeatPaymentState = firstSeatAddResponse.Value as PaymentStateModel;

            firstSeatPaymentState.ItemsAmount.Should().Be(1);
            firstSeatPaymentState.State.Should().Be(PaymentState.InProgress);

            // Add another seat to cart

            addCartRequestBody = new AddCartModel
            {
                EventId = mainEvent.Id,
                SeatId = secondSeat.Id,
                Price = secondSeat.Price,
                PriceOption = PriceOption.Adult,
            };

            var secondSeatAddResponse = await OrdersController.AddSeatToCart(CartId, addCartRequestBody) as OkObjectResult;
            var secondSeatPaymentState = secondSeatAddResponse.Value as PaymentStateModel;

            secondSeatPaymentState.ItemsAmount.Should().Be(2);
            secondSeatPaymentState.State.Should().Be(PaymentState.InProgress);

            // Confirm the choise (book seats in the Cart)

            var bookSeatsResponse = await OrdersController.BookSeatsInCart(CartId) as OkObjectResult;
            var paymentId = bookSeatsResponse.Value as string;
            PaymentsIds.Add(paymentId);

            // (Extra step) Verify the seats are booked

            var updatedEventSections = await GetEventSections(mainEvent.Id);
            var updatedSection = updatedEventSections.Find(es => es.Id == mainSection.Id);
            updatedSection.EventSeats.FirstOrDefault(seat => seat.Id == firstSeat.Id)
                .State.Should().Be(EventSeatState.Booked);
            updatedSection.EventSeats.FirstOrDefault(seat => seat.Id == secondSeat.Id)
                .State.Should().Be(EventSeatState.Booked);

            // Get & check Payment and its cart items

            var paymentResponse = await PaymentsController.GetPaymentStatus(paymentId) as OkObjectResult;
            (paymentResponse.Value as PaymentState?).Should().Be(PaymentState.InProgress);

            var cartItemsResult = await OrdersController.GetCartItems(CartId) as OkObjectResult;
            var cartItems = cartItemsResult.Value as CartItemDto[];

            var itemsCount = 2;

            cartItems.Should().HaveCount(itemsCount);

            cartItems.All(i => i.EventId == mainEvent.Id).Should().BeTrue();
            cartItems.All(i => i.EventSectionId == mainSection.Id).Should().BeTrue();
            cartItems.All(i => i.EventSectionClass == mainSection.Class).Should().BeTrue();
            cartItems.All(i => i.EventSectionNumber == mainSection.Number).Should().BeTrue();

            var cartItem = cartItems.FirstOrDefault(i => i.EventSeatId == firstSeat.Id);
            cartItem.EventRowNumber.Should().Be(firstSeat.RowNumber);
            cartItem.EventSeatId.Should().Be(firstSeat.Id);
            cartItem.EventSeatNumber.Should().Be(firstSeat.SeatNumber);
            cartItem.Price.Should().Be(firstSeat.Price);

            cartItem = cartItems.FirstOrDefault(i => i.EventSeatId == secondSeat.Id);
            cartItem.EventRowNumber.Should().Be(secondSeat.RowNumber);
            cartItem.EventSeatId.Should().Be(secondSeat.Id);
            cartItem.EventSeatNumber.Should().Be(secondSeat.SeatNumber);
            cartItem.Price.Should().Be(secondSeat.Price);

            // Update payment state

            EventSeatState updatedSeatState;
            PaymentState updatedPaymentState;
            if (paymentShouldBeSuccessful)
            {
                var successPaymentUpdateResponse = await PaymentsController.CompletePayment(paymentId) as OkResult;
                successPaymentUpdateResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

                updatedSeatState = EventSeatState.Sold;
                updatedPaymentState = PaymentState.Completed;
            }
            else
            {
                var failedPaymentUpdateResponse = await PaymentsController.FailPayment(paymentId) as OkResult;
                failedPaymentUpdateResponse.StatusCode.Should().Be(StatusCodes.Status200OK);

                updatedSeatState = EventSeatState.Available;
                updatedPaymentState = PaymentState.Failed;
            }

            // Verify cart items were deleted

            var actualPayment = await _paymentService.GetByIdAsync(paymentId);
            actualPayment.CartItems.Should().BeEmpty();

            // Verify Event Seats has matching state

            updatedEventSections = await GetEventSections(mainEvent.Id);
            updatedSection = updatedEventSections.FirstOrDefault(es => es.Id == mainSection.Id);

            updatedSection.EventSeats.FirstOrDefault(seat => seat.Id == firstSeat.Id)
                .State.Should().Be(updatedSeatState);
            updatedSection.EventSeats.FirstOrDefault(seat => seat.Id == secondSeat.Id)
                .State.Should().Be(updatedSeatState);

            // Verify Payment has updated state

            paymentResponse = await PaymentsController.GetPaymentStatus(paymentId) as OkObjectResult;
            var paymentUpdatedState = paymentResponse.Value as PaymentState?;
            paymentUpdatedState.Should().Be(updatedPaymentState);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GivenPaymentProcess_WhenCartItemIsEmpty_ShouldReturnBadRequest(string cartId)
        {
            await Setup();

            // Acts & Assertions

            // Get all events
            var eventsResponseResult = await EventsController.GetEvents() as OkObjectResult;
            var events = eventsResponseResult.Value as IReadOnlyCollection<EventDto>;
            var mainEvent = events.FirstOrDefault();

            // Get all events sections of the event

            var eventsSectionsResponseResult = await EventsController.GetEventsSections(mainEvent.Id) as OkObjectResult;
            var eventsSections = eventsSectionsResponseResult.Value as List<EventSectionDto>;
            var mainSection = eventsSections.FirstOrDefault();
            var freeSeats = mainSection.EventSeats.Where(s => s.State == EventSeatState.Available).ToList();
            var firstSeat = freeSeats[0];

            // Add one seat to cart

            var addCartRequestBody = new AddCartModel
            {
                EventId = mainEvent.Id,
                SeatId = firstSeat.Id,
                Price = firstSeat.Price,
                PriceOption = PriceOption.Adult,
            };

            var firstSeatAddResult = await OrdersController.AddSeatToCart(cartId, addCartRequestBody);
            var firstSeatAddResultValue = firstSeatAddResult as BadRequestObjectResult;

            firstSeatAddResultValue.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            firstSeatAddResultValue.Value.Should().NotBeNull();
        }

        private Task Setup(CancellationToken ct = default)
        {
            return GenerateEntities(ct);
        }

        private async Task<List<EventSectionDto>> GetEventSections(string eventId, string sectionId = "")
        {
            var updatedEventSectionsResponse = await EventsController.GetEventsSections(eventId) as OkObjectResult;
            return updatedEventSectionsResponse.Value as List<EventSectionDto>;
        }
    }
}
