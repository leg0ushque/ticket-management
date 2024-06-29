using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using TicketingSystem.Common.Exceptions;

namespace TicketingSystem.WebApi.Filters
{
    public class OutdatedVersionExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OutdatedVersionException)
            {
                int statusCode = (int)HttpStatusCode.BadRequest;

                string message = $"The data you have is outdated! Please, reload it before any further actions";

                var result = new ObjectResult(new
                {
                    message,
                    context.Exception.Source,
                    ExceptionType = context.Exception.GetType().FullName,
                })
                {
                    StatusCode = statusCode
                };

                context.Result = result;
            }
        }
    }
}
