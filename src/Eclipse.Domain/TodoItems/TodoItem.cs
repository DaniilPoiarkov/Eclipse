using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Domain.TodoItems;

public sealed class TodoItem : Entity
{
    private TodoItem(Guid id, Guid userId, string text, DateTime createdAt) : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates new TodoItem
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    /// <param name="createdAt"></param>
    /// <returns></returns>
    internal static Result<TodoItem> Create(Guid id, Guid userId, string? text, DateTime createdAt, bool isFinished, DateTime? finishedAt)
    {
        if (text.IsNullOrWhiteSpace() || text.Length < TodoItemConstants.MinLength)
        {
            return TodoItemDomainErrors.TodoItemIsEmpty();
        }

        if (text.Length > TodoItemConstants.MaxLength)
        {
            return TodoItemDomainErrors.TodoItemTooLong(TodoItemConstants.MaxLength);
        }

        return new TodoItem(id, userId, text, createdAt)
        {
            IsFinished = isFinished,
            FinishedAt = finishedAt
        };
    }

    private TodoItem() { }

    public Guid UserId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    /// <summary>Marks item as finished.</summary>
    public Result MarkAsFinished(DateTime finishedAt)
    {
        if (IsFinished)
        {
            return TodoItemDomainErrors.AlreadyFinished(Text);
        }

        IsFinished = true;
        FinishedAt = finishedAt;

        return Result.Success();
    }
}
