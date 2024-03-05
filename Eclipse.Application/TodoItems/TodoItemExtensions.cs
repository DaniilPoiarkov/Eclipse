using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Application.TodoItems;

internal static class TodoItemExtensions
{
    public static TodoItemDto ToDto(this TodoItem todoItem)
    {
        return new TodoItemDto
        {
            Id = todoItem.Id,
            UserId = todoItem.UserId,
            CreatedAt = todoItem.CreatedAt,
            FinishedAt = todoItem.FinishedAt,
            IsFinished = todoItem.IsFinished,
            Text = todoItem.Text,
        };
    }
}
