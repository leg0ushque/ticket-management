namespace TicketingSystem.BusinessLogic.Dtos
{
    public abstract class BaseDto
    {
        public string Id { get; set; }

        public long Version { get; set; }
    }
}
