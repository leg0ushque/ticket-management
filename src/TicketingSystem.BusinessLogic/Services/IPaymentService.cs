using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.Common.Enums;
using System.Collections.Generic;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IPaymentService : IService<Payment, PaymentDto>
    {
        public Task<Payment> GetIncompletePayment(string cartId);

        public Task<List<Payment>> GetCartPayments(string cartId);

        public Task UpdatePaymentState(string paymentId, PaymentState state);

        public Task<Payment> UpsertInProgressPayment(string cartId, string[] cartItemsIds,
            CancellationToken cancellationToken = default);
    }
}
