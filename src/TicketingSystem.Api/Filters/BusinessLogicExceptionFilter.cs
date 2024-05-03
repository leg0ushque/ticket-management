using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.Api.Filters
{
    public class BusinessLogicExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<BusinessLogicExceptionFilterAttribute> _logger;

        public override void OnException(ExceptionContext context)
        {
            if(context.Exception is BusinessLogicException bex && bex.Code == Common.Enums.ErrorCode.NotFound)
            {
                var result = new ObjectResult(new
                {
                    context.Exception.Message,
                    context.Exception.Source,
                    ExceptionType = context.Exception.GetType().FullName,
                })
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };

                string message = $"BusinessLogic exception with NotFound code occured: {context.Exception}";
                _logger.LogError(message);

                context.Result = result;
            }
        }
    }
}
