using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Enums;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.PurchasesApi.Models;

namespace TicketingSystem.PurchasesApi.Controllers
{
    /// <summary>
    /// API to manipulate over Cart Items
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IPaymentService _paymentService;
        private readonly ITicketService _ticketService;
        private readonly ICartItemService _cartItemService;
        private readonly IMapper _mapper;

        public OrdersController(
            ICartItemService cartItemService,
            ITicketService ticketService,
            IPaymentService paymentService,
            IEventService eventService,
            IEventSeatService eventSeatService,
            IMapper mapper)
        {
            _cartItemService = cartItemService;
            _ticketService = ticketService;
            _paymentService = paymentService;
            _eventService = eventService;
            _eventSeatService = eventSeatService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a list a list of items in a cart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("carts/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartItems([FromRoute] string cartId)
        {
            var items = await _cartItemService.FilterAsync(ci => ci.CartId == cartId);

            return Ok(items);
        }

        /// <summary>
        /// Adds a seat to the cart
        /// </summary>
        /// <returns>A cart state (with total amount)</returns>
        [HttpPost]
        [Route("carts/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSeatToCart([FromRoute] string cartId, [FromBody] AddCartModel model)
        {
            var existingEvent = await _eventService.GetByIdAsync(model.EventId);
            if(existingEvent is null)
            {
                return NotFound(nameof(model.EventId));
            }

            var existingSeatId = await _eventSeatService.GetByIdAsync(model.SeatId);
            if (existingSeatId is null)
            {
                return NotFound(nameof(model.SeatId));
            }

            return Ok(await AddSeatToCart(model, cartId));
        }

        /// <summary>
        /// Adds a seat to the cart. Returns Aacart state (with total amount)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSeatToCart([FromRoute] string cartId, [FromRoute] string eventId, [FromRoute] string seatId)
        {
            var existingEvent = await _eventService.GetByIdAsync(eventId);
            if(existingEvent is null)
            {
                return NotFound(nameof(eventId));
            }

            var existingSeatId = await _eventSeatService.GetByIdAsync(seatId);
            if (existingSeatId is null)
            {
                return NotFound(nameof(seatId));
            }

            return await DeleteSeatFromCart(seatId, cartId);
        }

        /// <summary>
        /// Moves all the seats in the cart to a booked state. Returns the ID of the related Payment
        /// </summary>
        [HttpPut]
        [Route("carts/{cartId}/book")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BookSeatsInCart([FromRoute] string cartId)
        {
            var seats = (await _cartItemService.FilterAsync(s => s.CartId == cartId))?.Select(ci => ci.EventSeatId).ToList();

            await _eventSeatService.UpdateEventSeatsStates(seats ?? Enumerable.Empty<string>().ToList(), EventSeatState.Booked);

            var paymentsOfCart = await _paymentService.FilterAsync(p => p.CartId == cartId);
            var payment = paymentsOfCart?.FirstOrDefault();

            if (paymentsOfCart == null || payment == null)
            {
                return NotFound(nameof(paymentsOfCart)); // TODO Verify that payment is created during other transactions
            }

            return Ok(payment.Id);
        }

        private async Task<SeatStateModel> AddSeatToCart(AddCartModel model, string cartId)
        {
            var ticket = new TicketDto
            {
                EventSeatId = model.SeatId,
                EventId = model.EventId,
                State = BusinessLogic.Enums.TicketState.NotPurchased,
                PriceOption = model.PriceOption,
                Price = model.Price,
                PurchasedOn = null,
                UserId = model.UserId,
            };

            await _ticketService.CreateAsync(ticket);

            var cartItem = new CartItemDto
            {
                CartId = cartId,
                TicketId = ticket.Id,
                EventSeatId = model.SeatId,
                CreatedOn = DateTimeOffset.UtcNow
            };

            await _cartItemService.CreateAsync(cartItem);

            var cartItems = await _cartItemService.FilterAsync(ci => ci.CartId == cartId);

            var payment = await _paymentService.UpsertInProgressPayment(cartId, cartItems.Select(x => x.Id).ToArray());

            return new SeatStateModel
            {
                Amount = cartItems.Count,
                State = _mapper.Map<PaymentState>(payment.State),
            };
        }

        private async Task<IActionResult> DeleteSeatFromCart(string seatId, string cartId)
        {
            var cartItem = (await _cartItemService.FilterAsync(ci =>
                ci.EventSeatId == seatId
                && ci.CartId == cartId)).FirstOrDefault();

            if (cartItem == null)
            {
                return NotFound();
            }
            else
            {
                await _cartItemService.DeleteAsync(cartItem.Id);

                return Ok();
            }
        }
    }
}
