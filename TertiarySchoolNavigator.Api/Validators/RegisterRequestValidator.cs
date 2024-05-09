
using FluentValidation;
using TertiarySchoolNavigator.Api.Contracts.Auth;

namespace TertiarySchoolNavigator.Api.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterUserRequest>

    {

        public RegisterRequestValidator()
        {
            RuleFor(x => x.Firstname).NotEmpty().WithMessage("Firstname is required.");
            RuleFor(x => x.Lastname).NotEmpty().WithMessage("Lastname is required.");
            RuleFor(x => x.Gender).NotEmpty().WithMessage("Gender is required.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
