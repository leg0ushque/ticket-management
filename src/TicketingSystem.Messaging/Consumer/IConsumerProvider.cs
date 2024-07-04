using Confluent.Kafka;
using System;
using TicketingSystem.Messaging.Models.Models;

namespace TicketingSystem.Messaging.Consumer
{
    public interface IConsumerProvider : IDisposable
    {
        IConsumer<string, MessageValue> Consumer { get; }
    }
}
