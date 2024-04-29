using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Payment : BaseEntity
    {
        public string[] CartItemIds { get; set; }

        public PaymentState State { get; set; }
    }
}
