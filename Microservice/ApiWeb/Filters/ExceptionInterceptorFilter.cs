using Microservice.ApiWeb.Dto.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microservice.ApiWeb.Filters
{
    public class ExceptionInterceptorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var ExceptionResponse = new ExceptionDtoResponse() {
                ErrorMessage = context.Exception.Message,
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorType = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
                Timestamp = DateTime.Now.ToString(),
                Failures = null
            };
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(ExceptionResponse);
            context.ExceptionHandled = true;
        }
    }
}
