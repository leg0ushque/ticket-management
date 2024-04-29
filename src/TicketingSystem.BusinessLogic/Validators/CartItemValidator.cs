using System;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class CartItemValidator : IValidator<CartItemDto>
    {
        private const int MinutesPrecision = 1;

        public void Validate(CartItemDto entity)
        {
            if ((entity.CreatedOn - DateTime.Now).TotalMinutes < MinutesPrecision)
            {
                throw new BusinessLogicException("CreatedOn cannot be in the future");
            }
        }
    }
}
