using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using TicketingSystem.DataAccess.Enums;

namespace TicketingSystem.DataAccess.Entities
{
    public class Ticket : BaseEntity
    {
        public string EventSeatId { get; set; }

        public string EventId { get; set; }

        public TicketState State { get; set; }

        public PriceOption PriceOption { get; set; }

        public decimal Price { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset PurchasedOn { get; set; }

        public string UserId { get; set; }
    }
}
