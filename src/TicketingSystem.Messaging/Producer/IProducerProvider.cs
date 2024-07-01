using com.ticketingSystem;
using Confluent.Kafka;
using System;

namespace TicketingSystem.Messaging.Producer
{
    public interface IProducerProvider : IDisposable
    {
        IProducer<string, MessageValue> Producer { get; }
    }
}
