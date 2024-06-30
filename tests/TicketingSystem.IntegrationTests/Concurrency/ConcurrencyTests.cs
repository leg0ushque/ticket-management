using AutoFixture;
using FluentAssertions;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Exceptions;
using TicketingSystem.DataAccess.Entities;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace TicketingSystem.IntegrationTests.Concurrency
{
    [Collection("Sequential")]
    public class ConcurrencyTests(DatabaseFixture fixture)
        : FixtureTestsBase(fixture), IClassFixture<DatabaseFixture>
    {
        private ConcurrentBag<string> successfulRequests = new();
        private ConcurrentBag<string> failedRequests = new();

        [Fact]
        public async Task GivenBookingTransaction_WhenExceptionOccured_ShouldBeRollbacked()
        {
            // Arrange
            var payments = await SetupPessimisticPayments(); // p1: #1-5, p2: #3

            // Book seat #3
            var secondGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[1]));
            await _eventSectionService.ExecuteBookingTransactionAsync(secondGroupedSeatsInfo);

            var updatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var bookedSeat = updatedSection.EventSeats.FirstOrDefault(es
                => es.Id == payments[1].CartItems[0].EventSeatId);

            bookedSeat.State.Should().Be(EventSeatState.Booked);

            // Act

            // Book seats #1-5
            var firstGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[0]));
            var action = () => _eventSectionService.ExecuteBookingTransactionAsync(firstGroupedSeatsInfo);

            // Assert

            await action.Should().ThrowAsync<OutdatedVersionException>();

            var firstPaymentSeats = payments[0].CartItems.Select(ci => ci.EventSeatId);
            var notUpdatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var notUpdatedSeats = notUpdatedSection.EventSeats.Where(es
                => firstPaymentSeats.Any(x => x == es.Id) && es.Id != bookedSeat.Id);

            notUpdatedSeats.All(s => s.State == EventSeatState.Available).Should().BeTrue();
        }

        [Fact]
        public async Task Given1000Carts_WhenRunBooking_ShouldBeHandledConcurrently()
        {
            const int requestsAmount = 1000;

            // Arrange
            var payments = await Setup(requestsAmount);

            var cartIds = payments.Select(p => p.CartId).ToList();

            // Act
            var tasks = new List<Task>();

            //for (int i = 0; i < requestsAmount; i++)
            //{
            //    tasks.Add(ExecuteEndpointCall(cartIds[i]));
            //}
            Parallel.For(0, requestsAmount, (i) =>
                tasks.Add(ExecuteEndpointCall(cartIds[i])));

            await Task.WhenAll(tasks);

            successfulRequests.Should().HaveCount(1);
            failedRequests.Should().HaveCount(requestsAmount - 1);
        }

        private async Task ExecuteEndpointCall(string cartId)
        {
            try
            {
                await OrdersController.BookSeatsInCartConcurrently(cartId);
                successfulRequests.Add(cartId);
            }
            catch (MongoCommandException)
            {
                failedRequests.Add(cartId);
            }
            catch (OutdatedVersionException)
            {
                failedRequests.Add(cartId);
            }
        }

        private async Task<List<Payment>> SetupPessimisticPayments(CancellationToken ct = default)
        {
            await GenerateEntities(ct);

            var eventId = EventsIds.FirstOrDefault();
            var eventSectionId = EventSectionsIds.FirstOrDefault();
            var eventSection = await _dbFixture.EventSectionRepositoryInstance.GetByIdAsync(eventSectionId, ct);

            List<EventSeat> firstEventSeatsList =
                [
                eventSection.EventSeats[0],
                eventSection.EventSeats[1],
                eventSection.EventSeats[2],
                eventSection.EventSeats[3],
                eventSection.EventSeats[4],
                ];

            List<EventSeat> secondEventSeatsList =
                [
                    eventSection.EventSeats[2]
                ];

            var firstPayment = fixture.Build<Payment>()
                .With(p => p.CartId, Guid.NewGuid().ToString())
                .With(p => p.State, PaymentState.InProgress)
                .With(p => p.LastUpdatedOn, DateTime.Now)
                .With(p => p.CartItems,
                    firstEventSeatsList.Select(es => new CartItem
                    {
                        EventId = eventId,
                        EventSectionId = eventSectionId,
                        EventSectionClass = eventSection.Class,
                        EventSectionNumber = eventSection.Number,
                        EventSeatId = es.Id,
                        EventRowNumber = es.RowNumber,
                        EventSeatNumber = es.SeatNumber,
                        Price = 5.5m,
                    }).ToArray()
                ).Create();

            var secondPayment = fixture.Build<Payment>()
                .With(p => p.CartId, Guid.NewGuid().ToString())
                .With(p => p.State, PaymentState.InProgress)
                .With(p => p.LastUpdatedOn, DateTime.Now)
                .With(p => p.CartItems,
                    secondEventSeatsList.Select(es => new CartItem
                    {
                        EventId = eventId,
                        EventSectionId = eventSectionId,
                        EventSectionClass = eventSection.Class,
                        EventSectionNumber = eventSection.Number,
                        EventSeatId = es.Id,
                        EventRowNumber = es.RowNumber,
                        EventSeatNumber = es.SeatNumber,
                        Price = 5.5m,
                    }).ToArray()
                ).Create();

            List<Payment> paymentsToCreate = [firstPayment, secondPayment];

            PaymentsIds = await CreateEntities(_dbFixture.PaymentRepositoryInstance, paymentsToCreate, ct);

            return [.. paymentsToCreate];
        }

        private async Task<List<Payment>> Setup(int amount, CancellationToken ct = default)
        {
            await GenerateEntities(ct);

            var eventId = EventsIds.FirstOrDefault();
            var eventSectionId = EventSectionsIds.FirstOrDefault();
            var eventSection = await _dbFixture.EventSectionRepositoryInstance.GetByIdAsync(eventSectionId, ct);
            var eventSeat = eventSection.EventSeats.FirstOrDefault();

            var paymentsToCreate = fixture.Build<Payment>()
                .With(p => p.CartId)
                .With(p => p.State, PaymentState.InProgress)
                .With(p => p.LastUpdatedOn, DateTime.Now)
                .With(p => p.CartItems,
                [
                    new CartItem
                    {
                        EventId = eventId,
                        EventSectionId = eventSectionId,
                        EventSectionClass = eventSection.Class,
                        EventSectionNumber = eventSection.Number,
                        EventSeatId = eventSeat.Id,
                        EventRowNumber = eventSeat.RowNumber,
                        EventSeatNumber = eventSeat.SeatNumber,
                        Price = 5.5m,
                    }
                ]).CreateMany(amount);

            PaymentsIds = await CreateEntities(_dbFixture.PaymentRepositoryInstance, paymentsToCreate, ct);

            return paymentsToCreate.ToList();
        }
    }
}
