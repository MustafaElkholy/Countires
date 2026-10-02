using Countries.Application.DTOs.Cities.Request;
using FluentValidation;

namespace Countries.Application.Validators.Cities
{
    public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
    {
        public CreateCityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CountryId)
                .GreaterThan(0);
        }
    }
}
