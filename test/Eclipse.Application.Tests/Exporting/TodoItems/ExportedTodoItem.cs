namespace Eclipse.Application.Tests.Exporting.TodoItems;

internal sealed class ExportedTodoItem : ExportedRow
{
    public Guid UserId { get; set; }
    public string? Text { get; set; }
    public bool IsFinished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
};
