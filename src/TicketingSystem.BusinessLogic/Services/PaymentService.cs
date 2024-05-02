using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class PaymentService(IMongoRepository<Payment> repository, IMapper mapper)
        : GenericEntityService<Payment, PaymentDto>(repository, mapper), IPaymentService
    {
        public Task UpdatePaymentState(string paymentId, PaymentState state)
        {
            return _repository.UpdateAsync(paymentId, p => p.State, _mapper.Map<PaymentState>(state));
        }

        public async Task<Payment> UpsertInProgressPayment(string cartId, string[] cartItemsIds, CancellationToken cancellationToken = default)
        {
            var existingPaymentResults = await _repository.FilterAsync(p =>
                    p.CartId == cartId
                    && p.State == PaymentState.InProgress,
                cancellationToken);
            var existingPayment = existingPaymentResults?.FirstOrDefault();

            if (existingPaymentResults == null || existingPayment == null)
            {
                var paymentToCreate = new Payment
                {
                    CartId = cartId,
                    CartItemIds = cartItemsIds,
                    State = PaymentState.InProgress
                };

                await _repository.CreateAsync(paymentToCreate, cancellationToken);

                return paymentToCreate;
            }
            else
            {
                existingPayment.CartItemIds = cartItemsIds;
                await _repository.UpdateAsync(existingPayment.Id, existingPayment, cancellationToken);

                return existingPayment;
            }
        }
    }
}
