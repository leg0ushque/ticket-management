using System;

namespace TicketingSystem.DataAccess.Entities
{
    public class Ticket : BaseEntity
    {
        public string EventSeatId { get; set; }

        public decimal Price { get; set; }

        public DateTime PurchasedOn { get; set; }
    }
}
