using FluentValidation;

namespace HyperPost.DTO.Pagination
{
    public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
    {
        public PaginationRequestValidator()
        {
            RuleFor(x => x.Page).NotEmpty().WithMessage("Page is required.");
            RuleFor(x => x.Limit).NotEmpty().WithMessage("Limit is required.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1.");
            RuleFor(x => x.Limit)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Limit must be greater than or equal to 1.");
        }
    }
}
