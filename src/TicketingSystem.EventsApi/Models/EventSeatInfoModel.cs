using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.EventsApi.Models
{
    public class EventSeatInfoModel
    {
        public string SectionId { get; set; }

        public string RowId { get; set; }

        public string SeatId { get; set; }

        public EventSeatState State { get; set; }

        public PriceOption PriceOption { get; set; }
    }
}
