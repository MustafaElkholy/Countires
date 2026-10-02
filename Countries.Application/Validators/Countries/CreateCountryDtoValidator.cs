using Countries.Application.DTOs.Countries.Request;
using FluentValidation;

namespace Countries.Application.Validators.Countries
{
    public class CreateCountryDtoValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches(@"\A[A-Za-z]{2}\z")
                .WithMessage("Country code must contain exactly two letters.");
        }
    }
}
