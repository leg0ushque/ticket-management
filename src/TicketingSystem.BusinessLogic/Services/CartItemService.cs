using AutoMapper;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class CartItemService : GenericEntityService<CartItem, CartItemDto>, ICartItemService
    {
        public CartItemService(IMongoRepository<CartItem> repository, IMapper mapper) : base(repository, mapper)
        { }
    }
}
