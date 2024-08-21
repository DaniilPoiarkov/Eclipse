using Eclipse.Application.Contracts.Exporting;

namespace Eclipse.Application.Exporting.TodoItems;

public sealed class ImportTodoItemDto : ImportEntityBase
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsFinished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}
