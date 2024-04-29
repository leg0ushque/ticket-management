namespace TicketingSystem.BusinessLogic.Dtos
{
    public class VenueDto : IDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
