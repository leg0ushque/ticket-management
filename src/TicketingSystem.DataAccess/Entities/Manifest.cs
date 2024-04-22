namespace TicketingSystem.DataAccess.Entities
{
    public class Manifest : BaseEntity
    {
        public string VenueId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
