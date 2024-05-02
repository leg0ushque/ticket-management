using TicketingSystem.Common.Enums;

namespace TicketingSystem.EventsApi.Models
{
    public class EventSeatInfoModel
    {
        public string EventSectionId { get; set; }

        public string EventSectionClass { get; set; }

        public int EventSectionNumber { get; set; }

        public int EventRowNumber { get; set; }

        public decimal EventRowPrice { get; set; }

        public int EventSeatNumber { get; set; }

        public EventSeatState EventSeatState { get; set; }
    }
}
