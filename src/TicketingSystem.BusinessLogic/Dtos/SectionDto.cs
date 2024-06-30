namespace TicketingSystem.BusinessLogic.Dtos
{
    public class SectionDto : BaseDto, IDto
    {
        public string VenueId { get; set; }

        public string Class { get; set; }

        public int Number { get; set; }

        public RowDto[] Rows { get; set; }
    }
}
