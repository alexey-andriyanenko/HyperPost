using FluentValidation;

namespace HyperPost.DTO.Package
{
    public class CreatePackageRequestValidator : AbstractValidator<CreatePackageRequest>
    {
        public CreatePackageRequestValidator()
        {
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId is required");

            RuleFor(x => x.SenderUserId).NotEmpty().WithMessage("SenderUserId is required");
            RuleFor(x => x.ReceiverUserId).NotEmpty().WithMessage("ReceiverUserId is required");
            RuleFor(x => x.ReceiverUserId)
                .NotEqual(x => x.SenderUserId)
                .WithMessage("SenderUserId and ReceiverUserId must be different");

            RuleFor(x => x.SenderDepartmentId)
                .NotEmpty()
                .WithMessage("SenderDepartmentId is required");
            RuleFor(x => x.ReceiverDepartmentId)
                .NotEmpty()
                .WithMessage("ReceiverDepartmentId is required");

            RuleFor(x => x.SenderDepartmentId)
                .NotEqual(x => x.ReceiverDepartmentId)
                .WithMessage("SenderDepartmentId and ReceiverDepartmentId must be different");

            RuleFor(x => x.PackagePrice).NotEmpty().WithMessage("PackagePrice is required");
            RuleFor(x => x.PackagePrice).PrecisionScale(8, 2, false);

            RuleFor(x => x.DeliveryPrice).NotEmpty().WithMessage("DeliveryPrice is required");
            RuleFor(x => x.DeliveryPrice)
                .GreaterThanOrEqualTo(5)
                .WithMessage("DeliveryPrice must be greater than 5");
            RuleFor(x => x.DeliveryPrice).PrecisionScale(8, 2, false);

            RuleFor(x => x.Weight).NotEmpty().WithMessage("Weight is required");
            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0.2m)
                .WithMessage("Weight must be greater than 0.2kg");
            RuleFor(x => x.Weight).PrecisionScale(4, 2, false);

            RuleFor(x => x.Description)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description is too long");
        }
    }
}
