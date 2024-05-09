
using FluentValidation;
using TertiarySchoolNavigator.Api.Contracts.Auth;

namespace TertiarySchoolNavigator.Api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginUserRequset>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.username).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
