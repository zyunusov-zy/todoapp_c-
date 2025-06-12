using FluentValidation;
using TodoApp.DTOs;

namespace TodoApp.Validators;

public class RegistrationDtoValidation : AbstractValidator<RegisterDto>
{
    public RegistrationDtoValidation()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username required")
            .MinimumLength(4).WithMessage("Username must contain at least 4 characters");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password required")
            .MinimumLength(6).WithMessage("Password must contain at least 6 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"\d").WithMessage("Must contain at least one number")
            .Matches(@"[^\w\d\s]").WithMessage("Must contain at least one symbol");
    }
}