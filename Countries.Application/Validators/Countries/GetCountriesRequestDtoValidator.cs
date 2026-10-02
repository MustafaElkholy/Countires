using Countries.Application.DTOs.Countries.Request;
using Countries.Application.Validators.Shared;
using FluentValidation;

namespace Countries.Application.Validators.Countries;

public class GetCountriesRequestDtoValidator : AbstractValidator<GetCountriesRequestDto>
{
    public GetCountriesRequestDtoValidator()
    {
        Include(new PaginationQueryDtoValidator());

        RuleFor(x => x.Name)
            .MaximumLength(100);
    }
}

