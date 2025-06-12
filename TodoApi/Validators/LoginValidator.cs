using FluentValidation;
using TodoApp.DTOs;

namespace TodoApp.Validators;

public class LoginDtoValidation : AbstractValidator<LoginDto>
{
    public LoginDtoValidation()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Not valid Email");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}