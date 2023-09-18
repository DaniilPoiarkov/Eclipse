using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Domain.Shared.TodoItems;

using FluentValidation;

namespace Eclipse.Application.TodoItems;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
{
    public CreateTodoItemValidator()
    {
        RuleFor(i => i.Text)
            .NotNull()
                .WithErrorCode(TodoItemError.Codes.Null)
                .WithMessage(TodoItemError.Messages.Empty)
            .NotEmpty()
                .WithErrorCode(TodoItemError.Codes.Empty)
                .WithMessage(TodoItemError.Messages.Empty)
            .MinimumLength(TodoItemConstants.MinLength)
                .WithErrorCode(TodoItemError.Codes.MinLength)
                .WithMessage(TodoItemError.Messages.Empty)
            .MaximumLength(TodoItemConstants.MaxLength)
                .WithErrorCode(TodoItemError.Codes.MaxLength)
                .WithMessage(TodoItemError.Messages.MaxLength);
    }
}
