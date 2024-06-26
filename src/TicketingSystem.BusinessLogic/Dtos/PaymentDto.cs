﻿using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class PaymentDto : IDto
    {
        public string Id { get; set; }

        public string CartId { get; set; }

        public PaymentState State { get; set; }

        public DateTimeOffset LastUpdatedOn { get; set; }

        public CartItemDto[] CartItems { get; set; }
    }
}
