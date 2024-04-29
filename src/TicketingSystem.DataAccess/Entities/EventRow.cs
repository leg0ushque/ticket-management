namespace TicketingSystem.DataAccess.Entities
{
    public class EventRow
    {
        public int Number { get; set; }

        public decimal Price { get; set; }

        public string [] EventSeatsIds { get; set; }
    }
}
