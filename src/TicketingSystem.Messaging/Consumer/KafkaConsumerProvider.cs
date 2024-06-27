using com.ticketingSystem;
using Confluent.Kafka;
using TicketingSystem.Messaging.Schema;

namespace TicketingSystem.Messaging.Consumer
{
    public class KafkaConsumerProvider : IConsumerProvider
    {
        private bool _disposed;

        public IConsumer<string, MessageValue> Consumer { get; }

        public KafkaConsumerProvider(KafkaConfigurationProvider config, IKafkaSchemaProvider schemaProvider)
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
