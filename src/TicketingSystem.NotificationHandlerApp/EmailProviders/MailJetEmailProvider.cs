using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.NotificationHandlerApp.HttpClients;
using TicketingSystem.NotificationHandlerApp.Models;

namespace TicketingSystem.NotificationHandlerApp.EmailProviders
{
    public class MailJetEmailProvider(IMailHttpClient mailHttpClient) : IEmailProvider
    {
        private readonly IMailHttpClient _mailHttpClient = mailHttpClient;

        public async Task<HttpResponseMessage> SendEmailAsync(EmailModel email, CancellationToken ct = default)
        {
            var result = await _mailHttpClient.SendEmailAsync(email, ct);

            string message = result.IsSuccessStatusCode ?
                $"Email was sent to {email.Recipients[0].Email} without exceptions"
                : $"Email wasn't sent to {email.Recipients[0].Email}, code: {result.StatusCode}";

            Console.WriteLine(message);

            return result;
        }

    }
}
