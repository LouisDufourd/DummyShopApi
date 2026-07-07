using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace DummyShopApi.API.DTO.Request
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
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
