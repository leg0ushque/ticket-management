namespace TicketingSystem.BusinessLogic.Dtos
{
    public class EventRowDto
    {
        public int Number { get; set; }

        public decimal Price { get; set; }

        public string[] EventSeatsIds { get; set; }
    }
}
