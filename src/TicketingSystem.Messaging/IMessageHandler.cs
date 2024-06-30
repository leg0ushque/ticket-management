using com.ticketingSystem;
using System.Threading;
using System.Threading.Tasks;

namespace TicketingSystem.Messaging
{
    public interface IMessageHandler
    {
        public Task HandleAsync(MessageValue message, CancellationToken ct = default);
    }
}
