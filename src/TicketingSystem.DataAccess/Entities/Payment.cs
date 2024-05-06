using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Payment : BaseEntity
    {
        public string CartId { get; set; }

        public PaymentState State { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset LastUpdatedOn { get; set; } = DateTimeOffset.Now;

        public CartItem[] CartItems { get; set; }
    }
}
