using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
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
    public class CartItemService(IMongoRepository<CartItem> cartItemRepository,
        IMongoRepository<Event> eventRepository,
        IMongoRepository<EventSeat> eventSeatRepository,
        IMongoRepository<Ticket> ticketRepository,
        IMapper mapper)
        : GenericEntityService<CartItem, CartItemDto>(cartItemRepository, mapper), ICartItemService
    {
        private readonly IMongoRepository<Event> _eventRepository = eventRepository;
        private readonly IMongoRepository<EventSeat> _eventSeatRepository = eventSeatRepository;
        private readonly IMongoRepository<Ticket> _ticketRepository = ticketRepository;

        public async Task<List<CartItemDto>> GetItemsOfCart(string cartId, CancellationToken cancellationToken = default)
        {
            return [.. _mapper.Map<List<CartItemDto>>(await _repository.FilterAsync(ci => ci.CartId == cartId, cancellationToken))];
        }

        public async Task AddSeatToCart(string cartId, string eventId, string seatId, decimal price, PriceOption priceOption, string userId, CancellationToken cancellationToken = default)
        {
            await _eventRepository.GetByIdAsync(eventId, cancellationToken);

            await _eventSeatRepository.GetByIdAsync(seatId, cancellationToken);

            var ticket = new Ticket
            {
                EventSeatId = seatId,
                EventId = eventId,
                State = TicketState.NotPurchased,
                PriceOption = priceOption,
                Price = price,
                PurchasedOn = null,
                UserId = userId,
            };

            await _ticketRepository.CreateAsync(ticket, cancellationToken);

            var cartItem = new CartItem
            {
                CartId = cartId,
                TicketId = ticket.Id,
                EventSeatId = seatId,
                CreatedOn = DateTimeOffset.UtcNow
            };

            await _repository.CreateAsync(cartItem, cancellationToken);
        }

        public async Task DeleteSeatFromCart(string eventId, string seatId, string cartId, CancellationToken cancellationToken = default)
        {
            await _eventRepository.GetByIdAsync(eventId, cancellationToken);

            await _eventSeatRepository.GetByIdAsync(seatId, cancellationToken);

            var cartItem = (await _repository.FilterAsync(ci =>
                ci.EventSeatId == seatId
                && ci.CartId == cartId, cancellationToken)).FirstOrDefault();

            if (cartItem == null)
            {
                throw new BusinessLogicException("Cart item not found", code: ErrorCode.NotFound);
            }
            else
            {
                await _repository.DeleteAsync(cartItem.Id, cancellationToken);
            }
        }

    }
}
