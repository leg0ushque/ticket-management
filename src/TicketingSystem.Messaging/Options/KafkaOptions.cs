namespace TicketingSystem.Messaging.Options
{
    public class KafkaOptions
    {
        public const string ConfigurationKey = "Kafka";

        public string ClientId { get; set; }

        public string Topic { get; set; }

        public string BootstrapServer { get; set; }
    }
}
