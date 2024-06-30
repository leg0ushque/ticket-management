using Newtonsoft.Json;

namespace TicketingSystem.NotificationHandlerApp.Models
{
    public class EmailRecipientModel
    {
        [JsonProperty("Html-part")]
        public string Email { get; set; }
    }
}
