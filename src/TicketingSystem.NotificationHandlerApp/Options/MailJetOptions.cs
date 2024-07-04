namespace TicketingSystem.NotificationHandlerApp.Options
{
    public class MailJetOptions
    {
        public static readonly string ConfigurationKey = "MailJet";

        public string AuthTokenValue { get; set; }

        public string ApiBaseAddress { get; set; }
    }
}
