using com.ticketingSystem;

namespace TicketingSystem.Messaging
{
    public interface IMessageHandler
    {
        public void Handle(MessageValue message);
    }
}
