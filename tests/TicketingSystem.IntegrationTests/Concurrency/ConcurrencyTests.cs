using AutoFixture;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Exceptions;
using TicketingSystem.DataAccess.Entities;
using Xunit;

namespace TicketingSystem.IntegrationTests.Concurrency
{
    public class ConcurrencyTests(DatabaseFixture fixture)
        : FixtureTestsBase(fixture), IClassFixture<DatabaseFixture>
    {
        [Fact]
        public async Task GivenBookingTransaction_WhenExceptionOccured_ShouldBeRollbacked()
        {
            // Arrange
            var payments = await SetupPessimisticPayments();
            var seatsForRollback = payments[1].CartItems.Select(x => x.EventSeatId).ToList();

            var cartIds = payments.Select(p => p.CartId).ToList();

            // Book seat #2
            var secondGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[0]));
            await _eventSectionService.ExecuteBookingTransactionAsync(secondGroupedSeatsInfo);

            var updatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var updatedSeat = updatedSection.EventSeats.FirstOrDefault(es
                => es.Id == payments[0].CartItems[0].EventSeatId);

            updatedSeat.State.Should().Be(EventSeatState.Booked);

            // Act

            // Book seats #1-5
            var firstGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[0]));
            var action = async () => await _eventSectionService.ExecuteBookingTransactionAsync(firstGroupedSeatsInfo);

            // Assert

            await action.Should().ThrowAsync<OutdatedVersionException>();

            var notUpdatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var notUpdatedSeats = notUpdatedSection.EventSeats.FirstOrDefault(es
                => es.Id == payments[0].CartItems[0].EventSeatId);

            updatedSeat.State.Should().Be(EventSeatState.Booked);

            foreach(var seatId in notUpdatedSeats)
            {
                var seat = updatedSection.EventSeats.FirstOrDefault(es
                    => es.Id == seatId);
                seat.State.Should().Be(EventSeatState.Available);
            }

            await TearDown();
        }

        [Fact]
        public async Task Given1000Carts_WhenRunBooking_ShouldBeHandledConcurrently()
        {
            // Arrange
            var payments = await Setup();

            var cartIds = payments.Select(p => p.CartId).ToList();

            // Book seat #2
            var secondGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[0]));
            await _eventSectionService.ExecuteBookingTransactionAsync(secondGroupedSeatsInfo);

            var updatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var updatedSeat = updatedSection.EventSeats.FirstOrDefault(es
                => es.Id == payments[0].CartItems[0].EventSeatId);

            updatedSeat.State.Should().Be(EventSeatState.Booked);

            // Act

            // Book seats #1-5
            var firstGroupedSeatsInfo = _paymentService.GetPaymentEventSeats(
                _mapper.Map<PaymentDto>(payments[0]));
            var action = async () => await _eventSectionService.ExecuteBookingTransactionAsync(firstGroupedSeatsInfo);

            // Assert

            action.Should().ThrowAsync<OutdatedVersionException>();

            var notUpdatedSection = await _eventSectionService.GetByIdAsync(EventSectionsIds[0]);
            var notUpdatedSeats = updatedSection.EventSeats.FirstOrDefault(es
                => es.Id == payments[0].CartItems[0].EventSeatId);

            updatedSeat.State.Should().Be(EventSeatState.Booked);

            foreach (var seatId in seatsForRollback)
            {
                var seat = updatedSection.EventSeats.FirstOrDefault(es
                    => es.Id == seatId);
                seat.State.Should().Be(EventSeatState.Available);
            }

            await TearDown();
        }

        //public async Task Given1000Carts_WhenRunBooking_ShouldBeHandledConcurrently()
        //{
        //    var payments = await Setup();
        //    var cartIds = payments.Select(p => p.CartId).ToList();

        //    // Acts & Assertions

        //    var tasksToRun = cartIds.Select(x =>
        //        Task.Run(async () =>
        //        {
        //            var response = await OrdersController.BookSeatsInCart(x);
        //        }))
        //        .ToList();
        //    // PLINQ!  - Parallel.For

        //    await Task.WhenAll(tasksToRun);

        //    // SOMETHING

        //    // Verify

        //    // Teardown & cleanup

        //    await TearDown();
        //}

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

        private async Task<List<Payment>> Setup(CancellationToken ct = default)
        {
            await GenerateEntities(ct);

            var eventId = EventsIds.FirstOrDefault();
            var eventSectionId = EventSectionsIds.FirstOrDefault();
            var eventSection = await _dbFixture.EventSectionRepositoryInstance.GetByIdAsync(eventSectionId, ct);
            var eventSeat = eventSection.EventSeats.FirstOrDefault();

            var paymentsToCreate = fixture.Build<Payment>()
                .With(p => p.CartId, Guid.NewGuid().ToString())
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
                ]).CreateMany(1000);

            PaymentsIds = await CreateEntities(_dbFixture.PaymentRepositoryInstance, paymentsToCreate, ct);

            return paymentsToCreate.ToList();
        }

        private Task TearDown(CancellationToken ct = default)
        {
            return DeleteGeneratedEntities(ct)
                .ContinueWith(x =>
                    RemoveEntities(_dbFixture.PaymentRepositoryInstance, PaymentsIds), ct);
        }

    }
}
