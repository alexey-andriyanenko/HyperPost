using FluentValidation;

namespace HyperPost.DTO.PackageCategory
{
    public class CreatePackageCategoryRequestValidator
        : AbstractValidator<CreatePackageCategoryRequest>
    {
        public CreatePackageCategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Name)
                .MaximumLength(30)
                .WithMessage("Name must be less than or equal to 30 characters");
        }
    }
}
