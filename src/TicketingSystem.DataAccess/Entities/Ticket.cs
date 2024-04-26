using System;
using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Ticket : BaseEntity
    {
        public string EventSeatId { get; set; }

        public string EventId { get; set; }

        public TicketState State { get; set; }

        public string PriceOptionId { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchasedOn { get; set; }

        public string UserId { get; set; }
    }
}
