using FluentValidation;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>

    {

        public RegisterRequestValidator()
        {
            RuleFor(x => x.RegisterFirstName).NotEmpty().WithMessage("RegisterFirstName is required.");
            RuleFor(x => x.RegisterFirstLastName).NotEmpty().WithMessage("RegisterFirstLastName is required.");
            RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
