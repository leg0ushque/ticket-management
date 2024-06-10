using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.Common.Enums;
using System.Collections.Generic;
using TicketingSystem.BusinessLogic.Models;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IPaymentService : IService<Payment, PaymentDto>
    {
        public List<EventSectionSeatsModel> GetPaymentEventSeats(PaymentDto payment);

        /// <summary>
        /// Deletes a seat from Cart or throws the exception if there are no seats in the cart.
        /// </summary>
        /// <exception cref="Exceptions.BusinessLogicException"></exception>
        public Task DeleteSeatFromCart(string seatId, string cartId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an existing payment with InProgress state or creates a new one with no Cart Items. Throws exception if provided cartId is null or empty
        /// </summary>
        /// <exception cref="Exceptions.BusinessLogicException"></exception>
        public Task<PaymentDto> GetIncompletePayment(string cartId, CancellationToken cancellationToken = default);

        public Task<List<PaymentDto>> GetCartPayments(string cartId, PaymentState state, CancellationToken cancellationToken = default);

        public Task UpdatePaymentState(string paymentId, PaymentState newState, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds CartItem to the existing InProgress payment or creates a new one with a single Cart Item
        /// </summary>
        public Task<PaymentStateModel> AppendCartItem(string cartId, string eventId, CartItemDto item, CancellationToken cancellationToken = default);
    }
}
