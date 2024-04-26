namespace TicketingSystem.DataAccess.Entities
{
    public class EventSection : BaseEntity
    {
        public string EventId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public EventRow[] EventRows { get; set; }
    }
}
