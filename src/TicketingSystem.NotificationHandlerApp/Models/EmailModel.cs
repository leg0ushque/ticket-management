using Newtonsoft.Json;

namespace TicketingSystem.NotificationHandlerApp.Models
{
    public class EmailModel
    {
        [JsonProperty("FromEmail")]
        public string FromEmail { get; set; }

        [JsonProperty("FromName")]
        public string FromName { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("Text-part")]
        public string TextPart { get; set; }

        [JsonProperty("Html-part")]
        public string HtmlPart { get; set; }

        [JsonProperty("Recipients")]
        public EmailRecipientModel[] Recipients { get; set; }
    }
}
