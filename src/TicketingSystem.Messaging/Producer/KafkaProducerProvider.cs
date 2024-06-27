using com.ticketingSystem;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketingSystem.Messaging.Schema;

namespace TicketingSystem.Messaging.Producer
{
    public class KafkaProducerProvider : IProducerProvider
    {
        private bool _disposed;

        public IProducer<string, MessageValue> Producer { get; }

        public KafkaProducerProvider(KafkaConfigurationProvider config, IKafkaSchemaProvider schemaProvider)
        {
            var avroSerializerConfig = new AvroSerializerConfig
            {
                BufferBytes = 1024
            };

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
