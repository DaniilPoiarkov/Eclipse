using Eclipse.Application.Contracts.TodoItems;
using FluentValidation;

namespace Eclipse.Application.TodoItems;

public class TodoItemValidator : AbstractValidator<TodoItemDto>
{
    public TodoItemValidator()
    {
        RuleFor(i => i.Text)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(125);
    }
}
