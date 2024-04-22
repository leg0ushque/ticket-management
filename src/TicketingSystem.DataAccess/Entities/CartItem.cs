﻿using System;

namespace TicketingSystem.DataAccess.Entities
{
    public class CartItem : BaseEntity
    {
        public string UserId { get; set; }

        public string TicketId { get; set; }

        public string EventSeatId { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
