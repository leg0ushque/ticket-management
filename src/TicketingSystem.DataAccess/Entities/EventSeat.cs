using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class EventSeat : BaseEntity
    {
        public string EventId { get; set; }

        public EventSeatState State { get; set; }

        public decimal Price { get; set; }
    }
}
