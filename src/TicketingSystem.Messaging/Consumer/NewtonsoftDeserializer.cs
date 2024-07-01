using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Text;

namespace TicketingSystem.Messaging.Consumer
{
    public class NewtonsoftKeyDeserializer<T> : IDeserializer<T>
        where T : class
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }

            var jsonString = Encoding.UTF8.GetString(data.ToArray());

            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
