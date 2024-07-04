using Confluent.Kafka;
using System;
using TicketingSystem.Messaging.Models.Models;

namespace TicketingSystem.Messaging.Producer
{
    public interface IProducerProvider : IDisposable
    {
        IProducer<string, MessageValue> Producer { get; }
    }
}
