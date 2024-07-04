using System.Threading.Tasks;
using System.Collections.Generic;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.BusinessLogic.Dtos;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IKafkaNotificationService
    {
        public Task CreatePaymentSucceededNotification(PaymentDto payment, List<EventSectionSeatsModel> groupedCartItems);
        public Task CreateSeatsBookedNotification(PaymentDto payment, List<EventSectionSeatsModel> groupedCartItems);
    }
}
