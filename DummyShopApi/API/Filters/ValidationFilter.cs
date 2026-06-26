using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DummyShopApi.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var argument = context.ActionArguments.Values.FirstOrDefault();
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = (IValidator) context.HttpContext.RequestServices.GetRequiredService(validatorType);
            var validatorContext = new ValidationContext<object>(argument);
            ValidationResult result = await validator.ValidateAsync(validatorContext);

            if(!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            await next();
        }
    }
}
