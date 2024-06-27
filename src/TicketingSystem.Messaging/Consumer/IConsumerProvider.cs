using com.ticketingSystem;
using Confluent.Kafka;

namespace TicketingSystem.Messaging.Consumer
{
    public interface IConsumerProvider : IDisposable
    {
        IConsumer<string, MessageValue> Consumer { get; }
    }
}
