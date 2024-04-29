using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class PaymentService : GenericEntityService<Payment, PaymentDto>, IPaymentService
    {
        public PaymentService(IMongoRepository<Payment> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
