using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventSeatDto : BaseDto, IDto
    {
        public int SeatNumber { get; set; }

        public int RowNumber { get; set; }

        public string CartId { get; set; }

        public decimal Price { get; set; }

        public string PaymentId { get; set; }

        public EventSeatState State { get; set; }

        // Other fields

        public string EventSectionId { get; set; }

        public string EventSectionClass { get; set; }

        public int EventSectionNumber { get; set;}
    }
}
