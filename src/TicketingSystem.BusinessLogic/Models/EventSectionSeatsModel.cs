namespace TicketingSystem.BusinessLogic.Models
{
    public class EventSectionSeatsModel
    {
        public string EventId { get; set; }

        public SectionSeatsModel[] SectionSeats { get; set; }
    }
}
