using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using TicketingSystem.BusinessLogic.Exceptions;

namespace TicketingSystem.WebApi.Filters
{
    public class BusinessLogicExceptionFilterAttribute : ExceptionFilterAttribute
    {
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

                context.Result = result;
            }
        }
    }
}
