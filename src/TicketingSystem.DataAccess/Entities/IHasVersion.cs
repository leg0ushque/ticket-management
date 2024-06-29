namespace TicketingSystem.DataAccess.Entities
{
    public interface IHasVersion
    {
        public long Version { get; set; }
    }
}
