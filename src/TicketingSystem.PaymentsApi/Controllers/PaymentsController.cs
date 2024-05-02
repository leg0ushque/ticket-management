using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.PaymentsApi.Controllers
{
    /// <summary>
    /// API to manipulate over Cart Items
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController(
        ICartItemService cartItemService,
        IPaymentService paymentService,
        IEventSeatService eventSeatService)
        : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly IEventSeatService _eventSeatService = eventSeatService;
        private readonly ICartItemService _cartItemService = cartItemService;

        /// <summary>
        /// Returns a statusof the payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentStatus([FromRoute] string paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            return payment is null ?
                NotFound(nameof(paymentId))
                : Ok(payment.State);
        }

        /// <summary>
        /// Updates payment status and moves all the seats related to a payment to the sold state.
        /// </summary>
        /// <returns></returns>
               [HttpPost]
        [Route("{paymentId}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompletePayment([FromRoute] string paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            if (payment is null)
            {
                return NotFound(nameof(paymentId));
            }

            var seatsUpdateStatus = await UpdatePaymentEventSeatsAsync(payment, EventSeatState.Sold);

            await _paymentService.UpdatePaymentState(payment.Id, PaymentState.Completed);

            return seatsUpdateStatus;
        }

        /// <summary>
        /// Updates payment status and moves all the seats related to a payment to the sold state.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("{paymentId}/failed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FailPayment([FromRoute] string paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            if (payment is null)
            {
                return NotFound(nameof(paymentId));
            }

            var seatsUpdateStatus = await UpdatePaymentEventSeatsAsync(payment, EventSeatState.Available);

            await _paymentService.UpdatePaymentState(payment.Id, PaymentState.Failed);

            return seatsUpdateStatus;
        }

        private async Task<IActionResult> UpdatePaymentEventSeatsAsync(PaymentDto payment, EventSeatState state)
        {
            var cartItems = await _cartItemService.FilterAsync(ci => ci.Id, payment.CartItemIds);

            if (cartItems is null)
            {
                return BadRequest();
            }

            var eventSeats = await _eventSeatService.FilterAsync(es => es.Id, cartItems.Select(c => c.EventSeatId).ToList());

            if (eventSeats is null || eventSeats.Count == 0)
            {
                return BadRequest();
            }

            var eventSeatsIds = eventSeats.Select(es => es.Id).ToList();

            await _eventSeatService.UpdateEventSeatsStates(eventSeatsIds, state);

            return Ok();
        }
    }
}
