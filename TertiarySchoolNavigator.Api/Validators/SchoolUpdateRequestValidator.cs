using FluentValidation;
using TertiarySchoolNavigator.Api.Contracts.School;

public class SchoolUpdateRequestValidator : AbstractValidator<SchoolUpdateRequest>
{
    public SchoolUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
     .NotEmpty()
     .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters.");

        RuleFor(x => x.Region)
            .NotEmpty()
            .WithMessage("Region is required.")
            .Length(1, 100)
            .WithMessage("Region must be between 1 and 100 characters.");

        RuleFor(x => x.District)
            .NotEmpty()
            .WithMessage("District is required.")
            .Length(1, 100)
            .WithMessage("District must be between 1 and 100 characters.");

        RuleFor(x => x.EstablishedYear)
            .InclusiveBetween(1800, 2100)
            .WithMessage("Established Year must be between 1800 and 2100.");

        RuleFor(x => x.SchoolType)
            .NotEmpty()
            .WithMessage("School Type is required.")
            .Length(1, 100)
            .WithMessage("School Type must be between 1 and 100 characters.");

    }
}