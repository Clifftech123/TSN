using FluentValidation;
using TertiarySchoolNavigator.Api.Models.SchoolModels;

public class SchoolUpdateRequestValidator : AbstractValidator<SchoolUpdateRequest>
{
    public SchoolUpdateRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().Length(1, 100);
        RuleFor(x => x.Region).NotEmpty().Length(1, 100);
        RuleFor(x => x.District).NotEmpty().Length(1, 100);
        RuleFor(x => x.EstablishedYear).InclusiveBetween(1800, 2100).When(x => x.EstablishedYear.HasValue);
        RuleFor(x => x.SchoolType).NotEmpty().Length(1, 100);
    }
}