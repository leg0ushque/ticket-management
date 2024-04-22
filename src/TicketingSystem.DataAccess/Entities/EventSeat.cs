using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class EventSeat : IStringKeyEntity
    {
        public string Id { get; set; }

        public string EventId { get; set; }

        public EventSeatState State { get; set; }

        public decimal Price { get; set; }
    }
}
