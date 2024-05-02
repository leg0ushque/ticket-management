using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Event : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset StartTime { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset EndTime { get; set; }

        public string VenueId { get; set; }

        public EventState State { get; set; }
    }
}
