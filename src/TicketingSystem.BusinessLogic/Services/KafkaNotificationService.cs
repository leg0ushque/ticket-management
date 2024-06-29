using com.ticketingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.Messaging.Models.Models;
using TicketingSystem.Messaging.Producer;

namespace TicketingSystem.BusinessLogic.Services
{
    public class KafkaNotificationService(IKafkaProducer kafkaProducer, INotificationService notificationService, IEventService eventService)
        : IKafkaNotificationService
    {
        private readonly IKafkaProducer _producer = kafkaProducer;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IEventService _eventService = eventService;

        public Task CreateSeatsBookedNotification(PaymentDto payment, List<EventSectionSeatsModel> groupedCartItems)
        {
            return CreateNotification(OperationType.SeatsBooked, payment, groupedCartItems);
        }

        public Task CreatePaymentSucceededNotification(PaymentDto payment, List<EventSectionSeatsModel> groupedCartItems)
        {
            return CreateNotification(OperationType.PaymentSucceeded, payment, groupedCartItems);
        }

        private async Task CreateNotification(OperationType operation, PaymentDto payment, List<EventSectionSeatsModel> groupedCartItems)
        {
            var notificationId = await _notificationService.CreateNotification(payment.Id);

            var seatsAmount = groupedCartItems.Sum(x => x.SectionSeats.Sum(x => x.SeatIds.Length));
            var seatsInfo = payment.CartItems.Select(x => new
            {
                x.EventId,
                Info = $"{x.EventSectionClass}{x.EventSectionNumber} - seat {x.EventSeatNumber}"
            });

            var groupedSeats = seatsInfo.GroupBy(x => x.EventId);

            var info = new StringBuilder();

            foreach (var one in groupedSeats)
            {
                var ev = await _eventService.GetByIdAsync(one.Key);
                var eventInfo = $"{ev.Name} ({ev.StartTime.ToString("d")} - {ev.EndTime.ToString("d")})\n";

                info.Append(eventInfo);
                info.Append($"Info: {string.Join("; ", one.Select(x => x.Info).ToList())}");
            };

            var message = new Message(Guid.NewGuid().ToString(),
                new MessageValue
                {
                    Id = payment.Id,
                    TrackingId = notificationId,
                    Timestamp = DateTime.Now,
                    CustomerEmail = "hww89115@doolk.com",
                    CustomerName = "Ivanov I.I.",
                    Operation = (int)operation,
                    OrderAmount = seatsAmount,
                    OrderSummary = $"{info}\n" +
                                   $"Total cost: {payment.CartItems.Sum(ci => ci.Price)},"
                });

            await _producer.ProduceMessageAsync(message);
        }
    }
}
