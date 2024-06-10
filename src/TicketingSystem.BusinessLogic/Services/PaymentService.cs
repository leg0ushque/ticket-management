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
        public async Task UpdatePaymentState(string paymentId, PaymentState newState, CancellationToken cancellationToken = default)
        {
            var payment = await _repository.GetByIdAsync(paymentId, cancellationToken);

            if(payment.State == newState)
            {
                return;
            }

            payment.State = newState;
            if (newState is PaymentState.Completed or PaymentState.Failed)
            {
                payment.CartItems = [];
            }

            await _repository.UpdateAsync(paymentId, payment, cancellationToken);
        }

        public List<EventSectionSeatsModel> GetPaymentEventSeats(PaymentDto payment)
        {
            // Events with sections containing a list of seats to update
            return payment.CartItems
                .GroupBy(x => x.EventId)
                .Select(grp => new EventSectionSeatsModel
                {
                    EventId = grp.Key,
                    SectionSeats = grp
                        .GroupBy(x => x.EventSectionId)
                        .Select(secGrp => new SectionSeatsModel
                        {
                            SectionId = secGrp.Key,
                            SeatIds = secGrp.Select(x => x.EventSeatId).ToArray()
                        })
                        .ToArray()
                })
                .ToList();
        }

        public async Task<PaymentDto> GetIncompletePayment(string cartId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                throw new BusinessLogicException($"Non-empty CartId must be provided", code: ErrorCode.Other);
            }

            var payments = await GetCartPayments(cartId, PaymentState.InProgress, cancellationToken);

            if (payments != null && payments.Count != 0)
            {
                return payments.FirstOrDefault();
            }

            var newPayment = new Payment
            {
                CartId = cartId,
                CartItems = [],
                State = PaymentState.InProgress,
            };

            await _repository.CreateAsync(newPayment, cancellationToken);

            return _mapper.Map<PaymentDto>(newPayment);
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

            if (payment.CartItems == null || payment.CartItems.Length == 0)
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
                CartId = payment.CartId,
            };
        }

        public async Task DeleteSeatFromCart(string seatId, string cartId, CancellationToken cancellationToken = default)
        {
            var payment = await GetIncompletePayment(cartId, cancellationToken);

            if (payment.CartItems.Length == 0)
            {
                throw new BusinessLogicException("There are no seats in the cart");
            }

            var updatedCartItems = payment.CartItems.Where(ci => ci.EventSeatId != seatId).ToArray();

            await _repository.UpdateAsync(payment.Id, p => p.CartItems, _mapper.Map<CartItem[]>(updatedCartItems), cancellationToken);
        }
    }
}
