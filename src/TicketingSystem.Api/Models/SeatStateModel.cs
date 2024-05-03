using TicketingSystem.Common.Enums;

namespace TicketingSystem.WebApi.Models
{
    public class SeatStateModel
    {
        public PaymentState State { get; set; }

        public int Amount { get; set; }
    }
}
