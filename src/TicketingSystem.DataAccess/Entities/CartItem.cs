using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace TicketingSystem.DataAccess.Entities
{
    public class CartItem : BaseEntity
    {
        public string CartId { get; set; }

        public string TicketId { get; set; }

        public string EventSeatId { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset CreatedOn { get; set; }
    }
}