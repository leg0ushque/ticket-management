namespace TicketingSystem.DataAccess.Entities
{
    public class Seat : BaseEntity
    {
        public string RowId { get; set; }

        public int Number { get; set; }
    }
}
