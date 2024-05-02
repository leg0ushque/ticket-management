using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class EventSeat : BaseEntity
    {
        public int Number { get; set; }

        public EventSeatState State { get; set; }
    }
}
