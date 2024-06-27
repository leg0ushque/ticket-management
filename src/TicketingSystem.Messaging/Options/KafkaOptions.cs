namespace TicketingSystem.Messaging.Options
{
    public class KafkaOptions
    {
        public const string ConfigurationSection = "Kafka";

        public string ClientId { get; set; }

        public string Topic { get; set; }

        public string BootstrapServer { get; set; }

        public string SchemaServer { get; set; }

        public string SchemaApiKey { get; set; }

        public string SchemaApiSecret { get; set; }

        public string SaslUsername { get; set; }

        public string SaslPassword { get; set; }
    }
}
