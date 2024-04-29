using System;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class TicketValidator : IValidator<TicketDto>
    {
        private const int MinutesPrecision = 1;
        private const decimal MinPrice = 0m;

        public void Validate(TicketDto entity)
        {
            if ((entity.PurchasedOn - DateTime.Now).TotalMinutes < MinutesPrecision)
            {
                throw new BusinessLogicException("Purchase date cannot be in the future.");
            }

            if (entity.Price < MinPrice)
            {
                throw new BusinessLogicException("Purchase must have a non-negative price.");
            }
        }
    }
}
