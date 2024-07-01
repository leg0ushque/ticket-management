using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class CartItemDto : BaseDto, IDto
    {
        public string EventId { get; set; }

        public string EventSectionId { get; set; }

        public string EventSectionClass { get; set; }

        public int EventSectionNumber { get; set; }

        public string EventSeatId { get; set; }

        public int EventRowNumber { get; set; }

        public int EventSeatNumber { get; set; }

        public decimal Price { get; set; }
    }
}
