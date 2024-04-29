namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventSectionDto : IDto
    {
        public string Id { get; set; }

        public string EventId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public EventRowDto[] EventRows { get; set; }
    }
}
