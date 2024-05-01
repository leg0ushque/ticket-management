using System;
using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class TicketDto : IDto
    {
        public string Id { get; set; }

        public string EventSeatId { get; set; }

        public string EventId { get; set; }

        public TicketState State { get; set; }

        public PriceOption PriceOption { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset? PurchasedOn { get; set; }

        public string UserId { get; set; }
    }
}
