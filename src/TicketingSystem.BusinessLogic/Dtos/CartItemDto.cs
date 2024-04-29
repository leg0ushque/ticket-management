using System;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class CartItemDto : IDto
    {
        public string Id { get; set; }

        public string CartId { get; set; }

        public string TicketId { get; set; }

        public string EventSeatId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
