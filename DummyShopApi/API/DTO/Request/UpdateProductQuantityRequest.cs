using FluentValidation;

namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductQuantityRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductQuantityRequestValidator : AbstractValidator<UpdateProductQuantityRequest>
    {
        public UpdateProductQuantityRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The id must be positive");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The quantity must be positive");
        }
    }
}
