using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.WebApi.Models;
using Xunit;

namespace TicketingSystem.IntegrationTests.Concurrency
{
    public class ConcurrencyTests(DatabaseFixture fixture)
        : FixtureTestsBase(fixture), IClassFixture<DatabaseFixture>
    {
        public async Task Given1000Carts_WhenRunBooking_ShouldBeHandledConcurrently()
        {
            var payments = await Setup();
            var cartIds = payments.Select(p => p.CartId).ToList();

            // Acts & Assertions

            var tasksToRun = cartIds.Select(x =>
                Task.Run(async() =>
                {
                    var response = await OrdersController.BookSeatsInCart(x);
                }))
                .ToList();

            await Task.WhenAll(tasksToRun);

            // SOMETHING

            // Teardown & cleanup

            await TearDown();
        }

        private async Task<List<Payment>> Setup(CancellationToken ct = default)
        {
            await GenerateEntities(ct);

            var eventId = EventsIds.FirstOrDefault();
            var eventSectionId = EventSectionsIds.FirstOrDefault();
            var eventSection = await _dbFixture.EventSectionRepositoryInstance.GetByIdAsync(eventSectionId);
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
                ]).CreateMany(100);

            PaymentsIds = await CreateEntities(_dbFixture.PaymentRepositoryInstance, paymentsToCreate, ct);

            return paymentsToCreate.ToList();
        }

        private Task TearDown(CancellationToken ct = default)
        {
            return DeleteGeneratedEntities(ct)
                .ContinueWith(x => RemoveEntities(_dbFixture.PaymentRepositoryInstance, PaymentsIds), ct);
        }

    }
}
