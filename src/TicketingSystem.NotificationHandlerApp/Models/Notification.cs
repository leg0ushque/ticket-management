using System;

namespace TicketingSystem.NotificationHandlerApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int TrackingId { get; set; }
        public string OperationName { get; set; }
        public DateTime Timestamp { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public decimal OrderAmount { get; set; }
        public string OrderSummary { get; set; }
        public NotificationStatus Status { get; set; }
    }
}
