using Countries.Application.DTOs.Countries.Request;
using FluentValidation;

namespace Countries.Application.Validators.Countries;

public class UpdateCountryDtoValidator : AbstractValidator<UpdateCountryDto>
{
    public UpdateCountryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches("^[a-zA-Z]{2}$")
            .WithMessage("Country code must contain exactly two letters.");
    }
}