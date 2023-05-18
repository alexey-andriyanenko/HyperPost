using FluentValidation;

namespace HyperPost.DTO.Department
{
    public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentRequestValidator()
        {
            RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required");
            RuleFor(x => x.FullAddress).NotEmpty().WithMessage("FullAddress is required");
            RuleFor(x => x.FullAddress)
                .MaximumLength(100)
                .WithMessage("FullAddress must be less than or equal to 100 characters");
        }
    }
}
