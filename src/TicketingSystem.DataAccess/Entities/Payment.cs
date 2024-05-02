using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Payment : BaseEntity
    {
        public string CartId { get; set; }

        public string[] CartItemIds { get; set; }

        public PaymentState State { get; set; }
    }
}
