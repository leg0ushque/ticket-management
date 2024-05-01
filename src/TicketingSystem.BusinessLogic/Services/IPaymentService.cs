using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IPaymentService : IService<Payment, PaymentDto>
    {
        Task UpdatePaymentState(string paymentId, Enums.PaymentState state);

        Task<Payment> UpsertInProgressPayment(string cartId, string[] cartItemsIds, CancellationToken cancellationToken = default);
    }
}
