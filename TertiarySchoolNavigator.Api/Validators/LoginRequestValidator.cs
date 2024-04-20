
using FluentValidation;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequset>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.username).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
