using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Models
{
    public class EventSeatInfoModel
    {
        public string EventSectionId { get; set; }

        public string EventSectionClass { get; set; }

        public int EventSectionNumber { get; set; }

        public int RowNumber { get; set; }

        public int SeatNumber { get; set; }

        public decimal Price { get; set; }

        public EventSeatState EventSeatState { get; set; }
    }
}
