using com.ticketingSystem;
using Confluent.Kafka;

namespace TicketingSystem.Messaging.Consumer
{
    public class KafkaConsumerProvider : IConsumerProvider
    {
        private bool _disposed;

        public IConsumer<string, MessageValue> Consumer { get; }

        public KafkaConsumerProvider(KafkaConfigurationProvider config)
        {
            Consumer = new ConsumerBuilder<string, MessageValue>(config.ConsumerConfiguration)
                .SetKeyDeserializer(new NewtonsoftKeyDeserializer<string>())
                .SetValueDeserializer(new NewtonsoftKeyDeserializer<MessageValue>())
                .Build();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                Consumer.Dispose();
            }
            _disposed = true;
        }
    }
}
