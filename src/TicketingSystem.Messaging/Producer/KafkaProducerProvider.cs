using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using TicketingSystem.Messaging.Models.Models;

namespace TicketingSystem.Messaging.Producer
{
    public class KafkaProducerProvider : IProducerProvider
    {
        private bool _disposed;

        public IProducer<string, MessageValue> Producer { get; }

        public KafkaProducerProvider(KafkaConfigurationProvider config)
        {
            Producer = new ProducerBuilder<string, MessageValue>(config.ProducerConfiguration)
                .SetKeySerializer(new NewtonsoftSerializer<string>())
                .SetValueSerializer(new NewtonsoftSerializer<MessageValue>())
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
                Producer.Dispose();
            }
            _disposed = true;
        }
    }
}
