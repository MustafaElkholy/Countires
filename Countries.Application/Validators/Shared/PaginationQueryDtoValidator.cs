using Countries.Application.DTOs.Shared;
using FluentValidation;

namespace Countries.Application.Validators.Shared
{
    public class PaginationQueryDtoValidator : AbstractValidator<PaginationQueryDto>
    {
        public PaginationQueryDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x)
                .Must(x =>
                    x.PageNumber < 1 ||
                    x.PageSize < 1 ||
                    ((long)x.PageNumber - 1) * x.PageSize <= int.MaxValue)
                .WithMessage("The requested page is too large.");
        }
    }
}
