namespace TicketingSystem.DataAccess.Entities
{
    public interface IHasId : IHasVersion
    {
        public string Id { get; set; }
    }
}
