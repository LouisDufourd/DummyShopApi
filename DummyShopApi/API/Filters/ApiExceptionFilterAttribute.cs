using DummyShopApi.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DummyShopApi.API.Filters
{
    public class ApiExceptionFilterAttribute: ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(NotFoundEntityException), HandleNotFoundException }
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        public void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();

            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var detail = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
            };

            context.Result = new NotFoundObjectResult(detail);

            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            Console.WriteLine(context.Exception.StackTrace);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundEntityException;

            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception?.Message,

            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}
