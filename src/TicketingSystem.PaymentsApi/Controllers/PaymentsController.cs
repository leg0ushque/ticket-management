using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;

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
        /// Returns a status of the payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{paymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentStatus([FromRoute] string paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            return Ok(payment.State);
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
            return Ok(await UpdatePayment(paymentId, EventSeatState.Sold, PaymentState.Completed));
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
            return Ok(await UpdatePayment(paymentId, EventSeatState.Available, PaymentState.Failed));
        }

        private async Task<IActionResult> UpdatePayment(string paymentId, EventSeatState eventSeatsState, PaymentState paymentState)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            var seatsUpdateStatus = await UpdatePaymentEventSeatsAsync(payment, eventSeatsState);

            await _paymentService.UpdatePaymentState(payment.Id, paymentState);

            return seatsUpdateStatus;
        }

        private async Task<IActionResult> UpdatePaymentEventSeatsAsync(PaymentDto payment, EventSeatState state)
        {
            var cartItems = await _cartItemService.FilterAsync(ci => ci.Id, payment.CartItemIds);

            if (cartItems is null)
            {
                return BadRequest();
            }

            await _eventSeatService.UpdateEventSeatsStates(
                cartItems.Select(c => c.EventSeatId).ToList(),
                state);

            return Ok();
        }
    }
}
