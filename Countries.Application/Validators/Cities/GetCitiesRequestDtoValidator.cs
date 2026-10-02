using Countries.Application.DTOs.Cities.Request;
using Countries.Application.Validators.Shared;
using FluentValidation;

namespace Countries.Application.Validators.Cities
{
    public class GetCitiesRequestDtoValidator : AbstractValidator<GetCitiesRequestDto>
    {
        public GetCitiesRequestDtoValidator()
        {
            Include(new PaginationQueryDtoValidator());

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.CountryId)
                .GreaterThan(0)
                .When(x => x.CountryId.HasValue);
        }
    }
}
