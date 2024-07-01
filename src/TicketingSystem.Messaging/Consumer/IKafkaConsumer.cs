using System.Threading;
using System.Threading.Tasks;

namespace TicketingSystem.Messaging.Consumer
{
    public interface IKafkaConsumer
    {
        public Task ListenAsync(CancellationToken ct = default);
    }
}
