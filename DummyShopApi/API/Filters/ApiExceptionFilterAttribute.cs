using DummyShopApi.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DummyShopApi.API.Filters
{
    /// <summary>
    /// Global API exception filter used to convert application exceptions
    /// into standardized HTTP responses.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly Dictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiExceptionFilterAttribute"/> class
        /// and registers exception handlers for known exception types.
        /// </summary>
        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(NotFoundEntityException), HandleNotFoundException },
                { typeof(ValidationException), HandleValidationException },
                { typeof(InvalidLoginException), HandleInvalidLoginException }
            };
        }

        /// <summary>
        /// Called when an exception occurs during action execution.
        /// </summary>
        /// <param name="context">
        /// The exception context containing information about the thrown exception.
        /// </param>
        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        /// <summary>
        /// Handles an exception by executing a registered exception handler.
        /// Unknown exceptions are handled as internal server errors.
        /// </summary>
        /// <param name="context">
        /// The context containing the exception to handle.
        /// </param>
        public void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();

            _exceptionHandlers.TryGetValue(
                type,
                out Action<ExceptionContext>? action);

            if (action != null)
            {
                action.Invoke(context);
                return;
            }

            HandleUnknownException(context);
        }

        /// <summary>
        /// Handles FluentValidation exceptions and returns a 400 Bad Request
        /// response containing validation errors grouped by property name.
        /// </summary>
        /// <param name="context">
        /// The context containing the validation exception.
        /// </param>
        private void HandleValidationException(ExceptionContext context)
        {
            var exception = (ValidationException)context.Exception;

            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var details = new ValidationProblemDetails(errors)
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Handles unexpected exceptions and returns a generic
        /// 500 Internal Server Error response.
        /// </summary>
        /// <param name="context">
        /// The context containing the unhandled exception.
        /// </param>
        private static void HandleUnknownException(ExceptionContext context)
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

        /// <summary>
        /// Handles entity not found exceptions and returns a
        /// 404 Not Found response.
        /// </summary>
        /// <param name="context">
        /// The context containing the not found exception.
        /// </param>
        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundEntityException;

            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception?.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Handles authentication failures caused by invalid credentials
        /// and returns a 401 Unauthorized response.
        /// </summary>
        /// <param name="context">
        /// The context containing the invalid login exception.
        /// </param>
        private void HandleInvalidLoginException(ExceptionContext context)
        {
            var exception = context.Exception as InvalidLoginException;

            var detail = new ProblemDetails()
            {
                Type = "",
                Title = "Invalid username or password",
                Detail = exception?.Message
            };

            context.Result = new UnauthorizedObjectResult(detail);

            context.ExceptionHandled = true;
        }
    }
}