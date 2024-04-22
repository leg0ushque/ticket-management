namespace TicketingSystem.DataAccess.Entities
{
    public class Section : IStringKeyEntity
    {
        public string Id { get; set; }

        public string ManifestId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }
    }
}
