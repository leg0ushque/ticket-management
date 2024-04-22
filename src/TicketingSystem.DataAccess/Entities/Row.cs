namespace TicketingSystem.DataAccess.Entities
{
    public class Row : IStringKeyEntity
    {
        public string Id { get; set; }

        public string SectionId { get; set; }

        public int Number { get; set; }
    }
}
