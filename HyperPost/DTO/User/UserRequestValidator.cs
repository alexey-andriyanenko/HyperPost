using FluentValidation;
using HyperPost.Shared;

namespace HyperPost.DTO.User
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
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

            RuleFor(x => x.RoleId).NotEmpty().WithMessage("RoleId is required");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumber is required");
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("PhoneNumber must be less than or equal to 20 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .When(
                    x =>
                        x.RoleId == (int)UserRolesEnum.Admin
                        || x.RoleId == (int)UserRolesEnum.Manager
                )
                .WithMessage("Password is required");
            RuleFor(x => x.Password)
                .MaximumLength(30)
                .When(x => x.Password != null || x.Password != "")
                .WithMessage("Password must be less than or equal to 30 characters");
        }
    }
}
