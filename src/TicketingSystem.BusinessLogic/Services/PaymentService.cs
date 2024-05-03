using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
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

        public async Task<Payment> GetIncompletePayment(string cartId)
        {
            var payments = await GetCartPayments(cartId);

            var payment = payments?.Find(p => p.State == PaymentState.InProgress);

            if (payments == null || payment == null)
            {
                throw new BusinessLogicException($"No payment was found by CartId {cartId}", code: ErrorCode.NotFound); // TODO Verify that payment is created during other transactions
            }

            return payment;
        }

        public Task<List<Payment>> GetCartPayments(string cartId)
        {
            return _repository.FilterAsync(p => p.CartId == cartId);
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
