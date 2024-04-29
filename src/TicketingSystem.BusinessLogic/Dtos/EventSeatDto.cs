using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventSeatDto : IDto
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public EventSeatState State { get; set; }
    }
}
