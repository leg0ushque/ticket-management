namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventSectionDto : BaseDto, IDto
    {
        public string EventId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public EventSeatDto[] EventSeats { get; set; }
    }
}
