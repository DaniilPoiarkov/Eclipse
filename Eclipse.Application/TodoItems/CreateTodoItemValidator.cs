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
                .WithErrorCode(TodoItemErrors.Codes.Null)
                .WithMessage(TodoItemErrors.Messages.Empty)
            .NotEmpty()
                .WithErrorCode(TodoItemErrors.Codes.Empty)
                .WithMessage(TodoItemErrors.Messages.Empty)
            .MinimumLength(TodoItemConstants.MinLength)
                .WithErrorCode(TodoItemErrors.Codes.MinLength)
                .WithMessage(TodoItemErrors.Messages.Empty)
            .MaximumLength(TodoItemConstants.MaxLength)
                .WithErrorCode(TodoItemErrors.Codes.MaxLength)
                .WithMessage(TodoItemErrors.Messages.MaxLength);
    }
}
