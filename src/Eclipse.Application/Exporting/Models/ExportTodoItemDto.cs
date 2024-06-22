namespace Eclipse.Application.Exporting.Models;

internal record ExportTodoItemDto(Guid Id, Guid UserId, string? Text, bool IsFinished, DateTime CreatedAt, DateTime? FinishedAt);
