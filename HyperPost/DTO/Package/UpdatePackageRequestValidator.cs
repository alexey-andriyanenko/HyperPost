using FluentValidation;

namespace HyperPost.DTO.Package
{
    public class UpdatePackageRequestValidator : AbstractValidator<UpdatePackageRequest>
    {
        public UpdatePackageRequestValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Description)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description must be less than or equal to 50 characters");
        }
    }
}
