using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;

namespace TicketingSystem.BusinessLogic.Services
{
    public interface IPaymentService : IService<Payment, PaymentDto>
    { }
}
