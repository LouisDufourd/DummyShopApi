using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductQuantityRequest
    {
        [Required]
        [JsonRequired]
        public int Quantity { get; set; }
    }

    public class UpdateProductQuantityRequestValidator : AbstractValidator<UpdateProductQuantityRequest>
    {
        public UpdateProductQuantityRequestValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The quantity must be positive");
        }
    }
}
