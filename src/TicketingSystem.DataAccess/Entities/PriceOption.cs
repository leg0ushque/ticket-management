namespace TicketingSystem.DataAccess.Entities
{
    public class PriceOption : BaseEntity
    {
        public string Name { get; set; }

        public decimal Coefficient { get; set; }
    }
}