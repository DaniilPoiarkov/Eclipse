namespace Eclipse.Application.Exporting.TodoItems;

internal record ExportTodoItemDto(Guid Id, Guid UserId, string? Text, bool IsFinished, DateTime CreatedAt, DateTime? FinishedAt);
