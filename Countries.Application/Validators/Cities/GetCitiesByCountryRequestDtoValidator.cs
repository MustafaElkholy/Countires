using Countries.Application.DTOs.Cities.Request;
using Countries.Application.Validators.Shared;
using FluentValidation;

namespace Countries.Application.Validators.Cities
{
    public class GetCitiesByCountryRequestDtoValidator : AbstractValidator<GetCitiesByCountryRequestDto>
    {
        public GetCitiesByCountryRequestDtoValidator()
        {
            Include(new PaginationQueryDtoValidator());

            RuleFor(x => x.CountryId).GreaterThan(0);

        }
    }
}
