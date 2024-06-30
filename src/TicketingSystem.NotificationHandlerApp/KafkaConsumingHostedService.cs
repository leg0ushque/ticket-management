using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Messaging.Consumer;

namespace TicketingSystem.NotificationHandlerApp
{
    public class KafkaConsumingHostedService(IKafkaConsumer kafkaConsumer) : IHostedService
    {
        private readonly IKafkaConsumer _kafkaConsumer = kafkaConsumer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => _kafkaConsumer.ListenAsync(cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}