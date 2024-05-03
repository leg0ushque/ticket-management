using System.Threading.Tasks;
using System.Threading;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.Common.Enums;
using System.Collections.Generic;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface ICartItemService : IService<CartItem, CartItemDto>
    {
        public Task<List<CartItemDto>> GetItemsOfCart(string cartId, CancellationToken cancellationToken = default);

        public Task AddSeatToCart(string cartId, string eventId, string seatId, decimal price, PriceOption priceOption, string userId, CancellationToken cancellationToken = default);

        public Task DeleteSeatFromCart(string eventId, string seatId, string cartId, CancellationToken cancellationToken = default);
    }
}
