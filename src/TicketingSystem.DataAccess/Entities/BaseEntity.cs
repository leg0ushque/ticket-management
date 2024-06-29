using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TicketingSystem.DataAccess.Entities
{
    public abstract class BaseEntity : IHasId
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public long Version { get; set; }
    }
}
