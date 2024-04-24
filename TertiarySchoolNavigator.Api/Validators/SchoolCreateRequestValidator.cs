using FluentValidation;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

namespace TertiarySchoolNavigator.Api.Validators
{

    public class SchoolCreateRequestValidator : AbstractValidator<SchoolCreateRequest>
    {
        public SchoolCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("School name is required.");
            RuleFor(x => x.Region).NotEmpty().WithMessage("Region is required.");
            RuleFor(x => x.District).NotEmpty().WithMessage("District is required.");
            RuleFor(x => x.SchoolType).NotEmpty().WithMessage("School type is required.");
            RuleFor(x => x.EstablishedYear).NotEmpty().WithMessage("Established year is required.")
                .GreaterThanOrEqualTo(1800).WithMessage("Established year should be greater than or equal to 1800.")
                .LessThanOrEqualTo(DateTime.Now.Year).WithMessage($"Established year should be less than or equal to {DateTime.Now.Year}.");
        }
    }
}
