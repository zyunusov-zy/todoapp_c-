using FluentValidation;
using TodoApp.DTOs;


namespace TodoApp.Validation;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(t => t.Title).NotEmpty().WithMessage("Title required")
            .MaximumLength(100).WithMessage("Max Length of the Title 100 characters");   
    }
}