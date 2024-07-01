using System.Threading.Tasks;
using TicketingSystem.Messaging.Models.Models;

namespace TicketingSystem.Messaging.Producer
{
    public interface IKafkaProducer
    {
        public Task ProduceMessageAsync(Message message);
    }
}
