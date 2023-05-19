using FluentValidation;
using HyperPost.Shared;

namespace HyperPost.DTO.User
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            // NOTE: editing phone number is not yet supported

            RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.FirstName)
                .MaximumLength(30)
                .WithMessage("FirstName must be less than or equal to 30 characters");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.LastName)
                .MaximumLength(30)
                .WithMessage("LastName must be less than or equal to 30 characters");

            RuleFor(x => x.Email)
                .MaximumLength(50)
                .When(x => x.Email != null || x.Email != "")
                .WithMessage("Email must be less than or equal to 50 characters");
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => x.Email != null || x.Email != "")
                .WithMessage("Email is not valid");
        }
    }
}
