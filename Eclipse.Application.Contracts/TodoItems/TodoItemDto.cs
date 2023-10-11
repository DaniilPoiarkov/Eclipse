using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.TodoItems;

public class TodoItemDto : EntityDto
{
    public Guid UserId { get; set; }

    public long TelegramUserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsFinished { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}
