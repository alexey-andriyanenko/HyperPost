using FluentValidation;

namespace HyperPost.DTO.User
{
    public class CheckIfUserExistsRequestValidator : AbstractValidator<CheckIfUserExistsRequest>
    {
        public CheckIfUserExistsRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Either email or phone number must be provided");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Either email or phone number must be provided");
        }
    }
}
