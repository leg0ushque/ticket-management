using com.ticketingSystem;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;
using TicketingSystem.Messaging.Models.Models;
using TicketingSystem.Messaging.Options;

namespace TicketingSystem.Messaging.Producer
{
    public class KafkaProducer : IKafkaProducer
    {
        private static ILogger _logger;
        private readonly IProducerProvider _producerProvider;
        private readonly IOptions<KafkaOptions> _kafkaOptions;

        public KafkaProducer(IProducerProvider producerProvider,
                             IOptions<KafkaOptions> kafkaOptions,
                             ILogger logger)
        {
            _kafkaOptions = kafkaOptions;
            _logger = logger;
            _producerProvider = producerProvider;
        }

        public async Task ProduceMessageAsync(Message message)
        {
            try
            {
                var producer = _producerProvider.Producer;

                var confluentKafkaMessage = new Confluent.Kafka.Message<string, MessageValue>
                {
                    Key = message.Key,
                    Value = message.Value
                };

                _logger?.Information("Trying to produce a message for customer with email {Email}", message.Value.CustomerEmail);

                await producer.ProduceAsync(_kafkaOptions.Value.Topic, confluentKafkaMessage);

                producer.Flush();
            }

            catch (Exception e)
            {
                _logger?.Error(e.Message);
                _logger?.Error("Customer email:{Email}, summary '{Summary}'", message.Value.CustomerEmail, message.Value.OrderSummary);
            }

        }
    }
}
