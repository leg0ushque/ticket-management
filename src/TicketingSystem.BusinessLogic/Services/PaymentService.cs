using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Enums;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class PaymentService : GenericEntityService<Payment, PaymentDto>, IPaymentService
    {
        public PaymentService(IMongoRepository<Payment> repository, IMapper mapper) : base(repository, mapper)
        { }

        public async Task<Payment> UpsertInProgressPayment(string cartId, string[] cartItemsIds, CancellationToken cancellationToken = default)
        {
            var existingPaymentResults = await _repository.FilterAsync(p =>
                    p.CartId == cartId
                    && p.State == PaymentState.InProgress,
                cancellationToken);
            var existingPayment = existingPaymentResults.FirstOrDefault();

            if (existingPaymentResults == null)
            {
                var paymentToCreate = new Payment
                {
                    CartId = cartId,
                    CartItemIds = cartItemsIds,
                    State = PaymentState.InProgress
                };

                await _repository.CreateAsync(paymentToCreate);

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
