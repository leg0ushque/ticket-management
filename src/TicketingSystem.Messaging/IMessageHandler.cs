using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Messaging.Models.Models;

namespace TicketingSystem.Messaging
{
    public interface IMessageHandler
    {
        public Task HandleAsync(MessageValue message, CancellationToken ct = default);
    }
}
