using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.WebApi.Filters;
using TicketingSystem.WebApi.Models;

namespace TicketingSystem.WebApi.Controllers
{
    /// <summary>
    /// API to manipulate over Cart Items
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [BusinessLogicExceptionFilter]
    [OutdatedVersionExceptionFilter]
    public class OrdersController(
        IPaymentService paymentService,
        IEventSectionService eventSectionService)
        : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IEventSectionService _eventSectionService = eventSectionService;

        /// <summary>
        /// Returns a list a list of items in a cart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("carts/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartItems([FromRoute] string cartId)
        {
            var payment = await _paymentService.GetIncompletePayment(cartId);

            return Ok(payment.CartItems);
        }

        /// <summary>
        /// Adds a seat to the cart
        /// </summary>
        /// <returns>A cart state (with total amount)</returns>
        [HttpPost]
        [Route("carts/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddSeatToCart([FromRoute] string cartId, [FromBody] AddCartModel model)
        {
            if (model == null || model.EventId == null || model.SeatId == null)
            {
                return BadRequest("Expected not-null EventId and SeatId values");
            }

            if (string.IsNullOrEmpty(cartId))
            {
                return BadRequest("Expected not-null cartId");
            }

            var eventSection = await _eventSectionService.GetSectionBySeatIdAsync(model.SeatId, model.EventId);
            var eventSeat = eventSection.EventSeats.FirstOrDefault(x => x.Id == model.SeatId);

            var paymentStateModel = await _paymentService.AppendCartItem(cartId, model.EventId,
                new CartItemDto
                {
                    Id = Guid.NewGuid().ToString(),
                    EventId = model.EventId,
                    EventRowNumber = eventSeat.RowNumber,
                    EventSeatId = eventSeat.Id,
                    EventSeatNumber = eventSeat.SeatNumber,
                    EventSectionId = eventSection.Id,
                    EventSectionClass = eventSection.Class,
                    EventSectionNumber = eventSection.Number,
                    Price = model.Price
                });

            return Ok(paymentStateModel);
        }

        /// <summary>
        /// Adds a seat to the cart. Returns the cart state (with total amount)
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("carts/{cartId}/events/{eventId}/seats/{seatId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSeatFromCart([FromRoute] string cartId, [FromRoute] string eventId, [FromRoute] string seatId)
        {
            await _paymentService.DeleteSeatFromCart(seatId, cartId);

            await _eventSectionService.UpdateEventSeatState(seatId, eventId, EventSeatState.Available);

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
            var payment = await _paymentService.GetIncompletePayment(cartId);

            // Events with sections containing a list of seats to update
            var groupedCartItems = _paymentService.GetPaymentEventSeats(payment);

            foreach (var item in groupedCartItems)
            {
                await _eventSectionService.BookSeatsOfEvent(item.EventId, item.SectionSeats);
            }

            return Ok(payment.Id);
        }
    }
}
