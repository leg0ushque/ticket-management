using TicketingSystem.Common.Enums;

namespace TicketingSystem.Messaging.Models.Models
{
    public class MessageValue
    {
		public string Id { get; set; }
        public string TrackingId { get; set; }
        public OperationType Operation { get; set; }
        public DateTime? Timestamp { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public double OrderAmount { get; set; }
        public string OrderSummary { get; set; }
    }
}
