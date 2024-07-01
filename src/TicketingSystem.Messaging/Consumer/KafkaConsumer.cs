using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Messaging.Models.Models;
using TicketingSystem.Messaging.Options;

namespace TicketingSystem.Messaging.Consumer
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly ILogger _logger;
        private readonly IOptions<KafkaOptions> _kafkaOptions;
        private readonly IConsumerProvider _consumerProvider;
        private readonly IMessageHandler _handler;

        public KafkaConsumer(IMessageHandler handler,
            IOptions<KafkaOptions> kafkaOptions,
                             IConsumerProvider consumerProvider,
                             ILogger logger)
        {
            _kafkaOptions = kafkaOptions;
            _consumerProvider = consumerProvider;
            _logger = logger;
            _handler = handler;
        }

        public async Task ListenAsync(CancellationToken ct = default)
        {
            using var consumer = _consumerProvider.Consumer;
            consumer.Subscribe(_kafkaOptions.Value.Topic);

            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume();
                    var result = new Message(consumeResult.Message.Key, consumeResult.Message.Value);

                    _logger.Information("Consumed message with key {Key}", consumeResult.Message.Key);

                    await _handler.HandleAsync(result.Value, ct);
                }
                catch (Exception e)
                {
                    _logger.Error(e, e.Message);
                }
            }
        }
    }
}
