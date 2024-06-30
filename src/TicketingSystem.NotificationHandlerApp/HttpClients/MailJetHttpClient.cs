using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.NotificationHandlerApp.Models;
using TicketingSystem.NotificationHandlerApp.Options;

namespace TicketingSystem.NotificationHandlerApp.HttpClients
{
    public class MailJetHttpClient (HttpClient httpClient, IOptions<MailJetOptions> options) : HttpClient, IMailHttpClient
    {
        private HttpClient Client = httpClient;

        private readonly IOptions<MailJetOptions> _options = options;

        public async Task<HttpResponseMessage> SendEmailAsync(EmailModel email, CancellationToken ct = default)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _options?.Value?.AuthTokenValue);

            var response = await PostAsync("send", new StringContent(JsonConvert.SerializeObject(email)), ct);

            return response;
        }
    }
}
