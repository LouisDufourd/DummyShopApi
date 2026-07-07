using DummyShopApi.DAL.Entities;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DummyShopApi.API.DTO.Request
{
    public class UpdateProductStatusRequest
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string Status { get; set; }
    }

    public class UpdateProductStatusRequestValidator : AbstractValidator<UpdateProductStatusRequest>
    {
        public UpdateProductStatusRequestValidator()
        {
            var productStatus = Enum.GetNames(typeof(EOrderProductStatus));
            RuleFor(x => x.Status)
                .Must(status => Enum.TryParse<EOrderProductStatus>(status, ignoreCase: false, out _))
                .WithMessage($"The status needs to be one of {{{string.Join(", ", productStatus)}}}");
            RuleFor(x => x.ProductId)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The id must be positive");
        }
    }
}
