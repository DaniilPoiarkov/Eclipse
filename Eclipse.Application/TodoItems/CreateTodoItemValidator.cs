using Eclipse.Application.Contracts.TodoItems;
using FluentValidation;

namespace Eclipse.Application.TodoItems;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
{
    public CreateTodoItemValidator()
    {
        RuleFor(i => i.Text)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(125);
    }
}
