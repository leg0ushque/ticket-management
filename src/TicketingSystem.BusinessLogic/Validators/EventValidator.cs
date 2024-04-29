using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class EventValidator : IValidator<EventDto>
    {
        public void Validate(EventDto entity)
        {
            if (entity.StartTime > entity.EndTime)
            {
                throw new BusinessLogicException("Start time must be before the end time");
            }
        }
    }
}
