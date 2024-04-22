namespace TicketingSystem.DataAccess.Entities
{
    public class Seat : IStringKeyEntity
    {
        public string Id { get; set; }

        public string RowId { get; set; }

        public int Number { get; set; }
    }
}
