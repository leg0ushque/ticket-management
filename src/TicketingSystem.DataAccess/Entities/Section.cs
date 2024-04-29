namespace TicketingSystem.DataAccess.Entities
{
    public class Section : BaseEntity
    {
        public string VenueId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public Row[] Rows { get; set; }
    }
}
