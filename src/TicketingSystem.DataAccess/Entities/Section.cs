namespace TicketingSystem.DataAccess.Entities
{
    public class Section : BaseEntity
    {
        public string ManifestId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }
    }
}
