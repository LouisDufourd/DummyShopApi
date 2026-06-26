using FluentValidation;

namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductQuantityRequest
    {
        public int Quantity { get; set; }
    }

    public class UpdateProductQuantityRequestValidator : AbstractValidator<UpdateProductQuantityRequest>
    {
        public UpdateProductQuantityRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The quantity must be positive");
        }
    }
}
