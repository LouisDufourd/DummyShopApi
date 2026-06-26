using DummyShopApi.Domain.Exceptions;
using FluentValidation;
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
                { typeof(NotFoundEntityException), HandleNotFoundException },
                { typeof(ValidationException), HandleValidationException }
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

            HandleUnknownException(context);
        }

        private void HandleValidationException(ExceptionContext context)
        {
            // Retrieve the FluentValidation exception that was thrown by the validation filter.
            var exception = (ValidationException)context.Exception;

            // Group validation errors by property name and convert them to the format
            // expected by ValidationProblemDetails:
            // Dictionary<string, string[]>
            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            // Build an RFC 7807 compliant response containing all validation errors.
            var details = new ValidationProblemDetails(errors)
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            // Return a 400 Bad Request response with the validation details.
            context.Result = new BadRequestObjectResult(details);

            // Indicate that the exception has been handled and should not propagate further.
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

            Console.WriteLine(context.Exception.Message);
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
