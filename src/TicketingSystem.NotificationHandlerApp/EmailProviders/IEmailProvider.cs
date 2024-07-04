using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.NotificationHandlerApp.Models;

namespace TicketingSystem.NotificationHandlerApp.EmailProviders
{
    public interface IEmailProvider
    {
        public Task<HttpResponseMessage> SendEmailAsync(EmailModel email, CancellationToken ct = default);
    }
}
