using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IPaymentService : IService<Payment, PaymentDto>
    {
        Task UpdatePaymentState(string paymentId, PaymentState state);

        Task<Payment> UpsertInProgressPayment(string cartId, string[] cartItemsIds, CancellationToken cancellationToken = default);
    }
}
