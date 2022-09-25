using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Microservice.ApiWeb.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EnableHttpTrafficLimiterAttribute : Attribute, IActionFilter, IResultFilter, IExceptionFilter
    {
        public int MaxHttpRequestAllowedAtTheSameTime;
        private static volatile int _activeHttpRequestNumber = 0;
        public void OnActionExecuting(ActionExecutingContext context) {
            if (Interlocked.Increment(ref _activeHttpRequestNumber) <= this.MaxHttpRequestAllowedAtTheSameTime) {
                //Console.WriteLine("Entro: " + _activeHttpRequestNumber.ToString());
                return;
            }
            //Console.WriteLine("Non entro: " + _activeHttpRequestNumber.ToString());
            context.Result = new StatusCodeResult((int)HttpStatusCode.Conflict);
        }

        public void OnActionExecuted(ActionExecutedContext context){
            Interlocked.Decrement(ref _activeHttpRequestNumber);
        }

        public void OnException(ExceptionContext context) {
            Interlocked.Decrement(ref _activeHttpRequestNumber);
        }

        public void OnResultExecuting(ResultExecutingContext context) { }

        public void OnResultExecuted(ResultExecutedContext context) { }

        
    }
}
