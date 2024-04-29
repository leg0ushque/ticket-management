using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.BusinessLogic.Validators
{
    public class SectionValidator : IValidator<SectionDto>
    {
        private const int MinNumber = 1;

        public void Validate(SectionDto entity)
        {
            if (string.IsNullOrEmpty(entity.Class))
            {
                throw new BusinessLogicException("Event section must have a class (min. 1 symbol).");
            }

            if (entity.Number < MinNumber)
            {
                throw new BusinessLogicException("Event section must be a positive number.");
            }

            if (entity.Rows.Any(er => er.Number < MinNumber))
            {
                throw new BusinessLogicException("All event rows must have positive number.");
            }
        }
    }
}
