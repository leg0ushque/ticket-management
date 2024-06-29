using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Notification : BaseEntity
    {
        public NotificationStatus Status { get; set; }

        public string PaymentId { get; set; }


        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset LastUpdatedOn { get; set; } = DateTimeOffset.Now;
    }
}
