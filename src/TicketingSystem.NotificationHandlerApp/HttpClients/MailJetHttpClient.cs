using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.NotificationHandlerApp.Models;
using TicketingSystem.NotificationHandlerApp.Options;

namespace TicketingSystem.NotificationHandlerApp.HttpClients
{
    public class MailJetHttpClient (HttpClient client, IOptions<MailJetOptions> options) : HttpClient, IMailHttpClient
    {
        private readonly IOptions<MailJetOptions> _options = options;

        public async Task<HttpResponseMessage> SendEmailAsync(EmailModel email, CancellationToken ct = default)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _options?.Value?.AuthTokenValue);
            BaseAddress = new Uri(_options.Value.ApiBaseAddress);

            var response = await PostAsync("send", new StringContent(JsonConvert.SerializeObject(email)), ct);

            return response;
        }
    }
}
