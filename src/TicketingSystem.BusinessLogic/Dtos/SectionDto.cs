namespace TicketingSystem.BusinessLogic.Dtos
{
    public class SectionDto : IDto
    {
        public string Id { get; set; }

        public string VenueId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public RowDto[] Rows { get; set; }
    }
}
