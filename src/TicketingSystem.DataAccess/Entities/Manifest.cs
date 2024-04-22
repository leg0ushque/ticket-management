namespace TicketingSystem.DataAccess.Entities
{
    public class Manifest : IStringKeyEntity
    {
        public string Id { get; set; }

        public string VenueId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
