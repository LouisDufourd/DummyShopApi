using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DummyShopApi.API.DTO.Request
{
    public class LoginRequest
    {
        [Required]
        [JsonRequired]
        public string Username { get; set; }
        [Required]
        [JsonRequired]
        public string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor((x) => x.Username)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
