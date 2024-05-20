using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Models
{
    public class PaymentStateModel
    {
        public string CartId { get; set; }

        public PaymentState State { get; set; }

        public int ItemsAmount { get; set; }
    }
}
