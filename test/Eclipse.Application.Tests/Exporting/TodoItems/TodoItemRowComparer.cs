using Eclipse.Domain.TodoItems;

namespace Eclipse.Application.Tests.Exporting.TodoItems;

internal sealed class TodoItemRowComparer : IExportRowComparer<TodoItem, ExportedTodoItem>
{
    public bool Compare(TodoItem entity, ExportedTodoItem row)
    {
        var createdAt = new DateTime(
            entity.CreatedAt.Year,
            entity.CreatedAt.Month,
            entity.CreatedAt.Day,
            entity.CreatedAt.Hour,
            entity.CreatedAt.Minute,
            entity.CreatedAt.Second
        );

        var recievedCreatedAt = new DateTime(
            row.CreatedAt.Year,
            row.CreatedAt.Month,
            row.CreatedAt.Day,
            row.CreatedAt.Hour,
            row.CreatedAt.Minute,
            row.CreatedAt.Second
        );

        return entity.Id == row.Id
            && entity.Text == row.Text
            && entity.UserId == row.UserId
            && createdAt == recievedCreatedAt
            && entity.IsFinished == row.IsFinished
            && entity.FinishedAt == row.FinishedAt;
    }
}
