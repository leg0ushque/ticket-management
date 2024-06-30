using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.WebApi.Filters;

namespace TicketingSystem.WebApi.Controllers
{
    /// <summary>
    /// API to manipulate over Cart Items
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [OutdatedVersionExceptionFilter]
    public class PaymentsController(
        IPaymentService paymentService,
        IEventSectionService eventSectionService)
        : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;

        private readonly IEventSectionService _eventSectionService = eventSectionService;

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
            await UpdatePayment(paymentId, EventSeatState.Booked, EventSeatState.Sold, PaymentState.Completed);

            return Ok();
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
            await UpdatePayment(paymentId, EventSeatState.Booked, EventSeatState.Available, PaymentState.Failed);

            return Ok();
        }

        private async Task UpdatePayment(string paymentId, EventSeatState oldState, EventSeatState newState, PaymentState paymentState)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            // Events with sections containing a list of seats to update
            var groupedCartItems = _paymentService.GetPaymentEventSeats(payment);

            foreach (var item in groupedCartItems)
            {
                await _eventSectionService.UpdateEventSeatsStateAsync(item.EventId, item.SectionSeats, oldState, newState);
            }

            await _paymentService.UpdatePaymentState(payment.Id, paymentState);
        }
    }
}
