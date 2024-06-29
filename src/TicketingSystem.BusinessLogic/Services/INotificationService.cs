using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface INotificationService : IService<Notification, NotificationDto>
    {
        public Task<string> CreateNotification(string paymentId, CancellationToken ct = default);

        public Task UpdateNotificationStatus(string paymentId, NotificationStatus status, CancellationToken ct = default);
    }
}
