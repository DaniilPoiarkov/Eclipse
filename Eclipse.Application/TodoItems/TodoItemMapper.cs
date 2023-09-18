using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Domain.TodoItems;

namespace Eclipse.Application.TodoItems;

public class TodoItemMapper : IMapper<TodoItem, TodoItemDto>
{
    public TodoItemDto Map(TodoItem value)
    {
        return new TodoItemDto
        {
            Id = value.Id,
            CreatedAt = value.CreatedAt,
            FinishedAt = value.FinishedAt,
            IsFinished = value.IsFinished,
            TelegramUserId = value.TelegramUserId,
            Text = value.Text,
        };
    }
}
