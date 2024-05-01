﻿using TicketingSystem.BusinessLogic.Enums;

namespace TicketingSystem.PurchasesApi.Models
{
    public class SeatStateModel
    {
        public PaymentState State { get; set; }

        public int Amount { get; set; }
    }
}