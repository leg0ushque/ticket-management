using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.PurchasesApi.Models
{
    public class AddCartModel
    {
        public string EventId { get; set; }

        public string SeatId { get; set; }

        public decimal Price { get; set; }

        public PriceOption PriceOption { get; set; }

        public string UserId { get; set;}
    }
}
