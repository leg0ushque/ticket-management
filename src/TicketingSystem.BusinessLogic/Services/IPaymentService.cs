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
        public Task<List<EventSectionSeatsModel>> GetPaymentEventSeats(string paymentId, CancellationToken cancellationToken = default);

        public Task DeleteSeatFromCart(string seatId, string cartId, CancellationToken cancellationToken = default);

        public Task<PaymentDto> GetIncompletePayment(string cartId, CancellationToken cancellationToken = default);

        public Task<List<PaymentDto>> GetCartPayments(string cartId, PaymentState state, CancellationToken cancellationToken = default);

        public Task UpdatePaymentState(string paymentId, PaymentState state, CancellationToken cancellationToken = default);

        public Task<PaymentStateModel> AppendCartItem(string cartId, string eventId, CartItemDto item, CancellationToken cancellationToken = default);
    }
}
