using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class EventSeatValidator : IValidator<EventSeatDto>
    {
        private const int MinNumber = 1;

        public void Validate(EventSeatDto entity)
        {
            if (entity.Number < MinNumber)
            {
                throw new BusinessLogicException("Number should be positive");
            }
        }
    }
}
