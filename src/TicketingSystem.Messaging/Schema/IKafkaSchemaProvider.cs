using Confluent.SchemaRegistry;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.Messaging.Schema
{
    public interface IKafkaSchemaProvider : IDisposable
    {
        CachedSchemaRegistryClient SchemaConfig { get; }
    }

    public class KafkaSchemaProvider : IKafkaSchemaProvider
    {
        public CachedSchemaRegistryClient SchemaConfig { get; }

        private bool _disposed;

        public KafkaSchemaProvider(IOptions<KafkaOptions> kafkaOptions)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = kafkaOptions.Value.SchemaServer,
                BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                BasicAuthUserInfo = $"{kafkaOptions.Value.SchemaApiKey}:{kafkaOptions.Value.SchemaApiSecret}",
            };
            SchemaConfig = new CachedSchemaRegistryClient(schemaRegistryConfig);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                SchemaConfig.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
