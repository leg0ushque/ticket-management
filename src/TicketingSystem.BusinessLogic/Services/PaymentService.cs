using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.BusinessLogic.Models;
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

        public async Task<List<EventSectionSeatsModel>> GetPaymentEventSeats(string paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await GetByIdAsync(paymentId, cancellationToken);

            // Events with sections containing a list of seats to update
            var groupedCartItems = payment.CartItems
                .GroupBy(
                    i => new { i.EventId, i.EventSectionId },
                    (key, g) => new
                    {
                        key.EventId,
                        SectionSeats = new SectionSeatsModel
                        {
                            SectionId = key.EventSectionId,
                            SeatIds = g.Select(i => i.EventSeatId).ToArray()
                        }
                    }
                )
                .GroupBy(
                    i => i.EventId,
                    (key, g) => new EventSectionSeatsModel
                    {
                        EventId = key,
                        SectionSeats = g.Select(x => x.SectionSeats).ToArray()
                    }
                );

            return groupedCartItems.ToList();
        }

        public async Task<PaymentDto> GetIncompletePayment(string cartId, CancellationToken cancellationToken = default)
        {
            var payments = await GetCartPayments(cartId, PaymentState.InProgress, cancellationToken);

            if (payments == null || payments.Count == 0)
            {
                throw new BusinessLogicException($"No payment was found by CartId {cartId}", code: ErrorCode.NotFound); // TODO Verify that payment is created during other transactions
            }

            return _mapper.Map<PaymentDto>(payments.FirstOrDefault());
        }

        public async Task<List<PaymentDto>> GetCartPayments(string cartId, PaymentState state, CancellationToken cancellationToken = default)
        {
            var items = await _repository.FilterAsync(p => p.CartId == cartId && p.State == state, cancellationToken);

            return _mapper.Map<List<PaymentDto>>(items);
        }

        public async Task<PaymentStateModel> AppendCartItem(string cartId, string eventId, CartItemDto item, CancellationToken cancellationToken = default)
        {
            var payment = await GetIncompletePayment(cartId, cancellationToken);

            CartItem[] updatedCartItems;
            var newItem = _mapper.Map<CartItem>(item);

            if (payment.CartItems == null)
            {
                updatedCartItems = [newItem];
            }
            else
            {
                updatedCartItems = [.. _mapper.Map<CartItem[]>(payment.CartItems), newItem];
            }

            await _repository.UpdateAsync(payment.Id, p => p.CartItems, updatedCartItems, cancellationToken);

            return new PaymentStateModel
            {
                ItemsAmount = updatedCartItems.Length,
                State = payment.State,
            };
        }

        public async Task DeleteSeatFromCart(string seatId, string cartId, CancellationToken cancellationToken = default)
        {
            var payment = await GetIncompletePayment(cartId, cancellationToken);

            var updatedCartItems = payment.CartItems.Where(ci => ci.EventSeatId != seatId).ToArray();

            await _repository.UpdateAsync(payment.Id, p => p.CartItems, _mapper.Map<CartItem[]>(updatedCartItems), cancellationToken);
        }
    }
}
