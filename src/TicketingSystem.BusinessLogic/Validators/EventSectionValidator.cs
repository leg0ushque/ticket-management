using System.Linq;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class EventSectionValidator : IValidator<EventSectionDto>
    {
        private const int MinNumber = 1;
        private const decimal MinPrice = 0m;

        public void Validate(EventSectionDto entity)
        {
            if (string.IsNullOrEmpty(entity.Class))
            {
                throw new BusinessLogicException("Event section must have a class (min. 1 symbol).");
            }

            if (entity.Number < MinNumber)
            {
                throw new BusinessLogicException("Event section must be a positive number.");
            }

            if (entity.EventRows.Any(er => er.Number < MinNumber))
            {
                throw new BusinessLogicException("All event rows must have a positive number.");
            }

            if (entity.EventRows.Any(er => er.Price < MinPrice))
            {
                throw new BusinessLogicException("All event rows must have a non-negative price.");
            }
        }
    }
}
