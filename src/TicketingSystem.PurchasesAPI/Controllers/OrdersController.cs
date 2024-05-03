using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Api.Filters;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.PurchasesApi.Models;

namespace TicketingSystem.PurchasesApi.Controllers
{
    /// <summary>
    /// API to manipulate over Cart Items
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [BusinessLogicExceptionFilter]
    public class OrdersController(
        ICartItemService cartItemService,
        IPaymentService paymentService,
        IEventSeatService eventSeatService,
        IMapper mapper)
        : ControllerBase
    {
        private readonly IEventSeatService _eventSeatService = eventSeatService;
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ICartItemService _cartItemService = cartItemService;
        private readonly IMapper _mapper = mapper;

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
            await _cartItemService.AddSeatToCart(cartId, model.EventId, model.SeatId, model.Price, model.PriceOption, model.UserId);

            var cartItems = await _cartItemService.GetItemsOfCart(cartId); // already has newly created CartItem

            var payment = await _paymentService.UpsertInProgressPayment(cartId, cartItems.Select(x => x.Id).ToArray());

            return Ok(new SeatStateModel
            {
                Amount = cartItems.Count,
                State = _mapper.Map<PaymentState>(payment.State),
            });
        }

        /// <summary>
        /// Adds a seat to the cart. Returns Aacart state (with total amount)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSeatFromCart([FromRoute] string cartId, [FromRoute] string eventId, [FromRoute] string seatId)
        {
            await _cartItemService.DeleteSeatFromCart(eventId, seatId, cartId);

            return Ok();
        }

        /// <summary>
        /// Moves all the seats in the cart to a booked state. Returns the ID of the related Payment
        /// </summary>
        [HttpPut]
        [Route("carts/{cartId}/book")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BookSeatsInCart([FromRoute] string cartId)
        {
            await _eventSeatService.BookSeatsInCart(cartId);

            var payment = await _paymentService.GetIncompletePayment(cartId);

            return Ok(payment.Id);
        }
    }
}
