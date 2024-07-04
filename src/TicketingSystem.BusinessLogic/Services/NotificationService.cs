using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class NotificationService(IMongoRepository<Notification> repository) : INotificationService
    {
        private readonly IMongoRepository<Notification> _repository = repository;

        public async Task<string> CreateNotification(string paymentId, CancellationToken ct = default)
        {
            var notification = new Notification
            {
                PaymentId = paymentId,
                Status = NotificationStatus.InProgress,
            };

            await _repository.CreateAsync(notification, ct);

            return notification.Id;
        }

        public async Task UpdateNotificationStatus(string paymentId, NotificationStatus status, CancellationToken ct = default)
        {
            var items = await _repository.FilterAsync(x => x.PaymentId == paymentId, ct);

            var item = items.FirstOrDefault() ??
                throw new BusinessLogicException(
                    $"Notification was not found by paymentId {paymentId}",
                    code: ErrorCode.NotFound);

            item.Status = status;

            await _repository.UpdateAsync(item.Id, item, ct);
        }
    }
}
