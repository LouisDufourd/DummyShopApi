using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DummyShopApi.API.DTO.Request
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        [JsonRequired]
        public int Id { get; set; }
        [Required]
        [JsonRequired]
        public string Status { get; set; }
    }

    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusRequestValidator()
        {
            var status = new List<string> { "Panier", "Payée", "Expédiée", "Livrée", "Annulée"};
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(status.Contains);

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("The id must be positive");
        }
    }
}