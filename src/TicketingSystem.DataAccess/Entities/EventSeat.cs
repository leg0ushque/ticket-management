using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class EventSeat
    {
        // Inline entity, doesn't have index but has Id
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int SeatNumber { get; set; }

        public int RowNumber { get; set; }

        public string CartId { get; set; }

        public decimal Price { get; set; }

        public string PaymentId { get; set; }

        public EventSeatState State { get; set; }
    }
}
