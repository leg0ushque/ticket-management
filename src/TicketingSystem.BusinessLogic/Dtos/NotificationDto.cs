using System;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Dtos
{
    public class NotificationDto : BaseDto, IDto
    {
        public NotificationStatus Status { get; set; }

        public string PaymentId { get; set; }

        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}
