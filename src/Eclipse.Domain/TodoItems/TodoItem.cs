using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;

namespace Eclipse.Domain.TodoItems;

public sealed class TodoItem : Entity
{
    //[JsonConstructor]
    private TodoItem(Guid id, Guid userId, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null) : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = createdAt;
        IsFinished = isFinished;
        FinishedAt = finishedAt;
    }

    /// <summary>
    /// Creates new TodoItem
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    /// <param name="createdAt"></param>
    /// <returns></returns>
    internal static Result<TodoItem> Create(Guid id, Guid userId, string? text, DateTime createdAt)
    {
        if (string.IsNullOrEmpty(text) || text.Length < TodoItemConstants.MinLength)
        {
            return TodoItemDomainErrors.TodoItemIsEmpty();
        }

        if (text.Length > TodoItemConstants.MaxLength)
        {
            return TodoItemDomainErrors.TodoItemTooLong(TodoItemConstants.MaxLength);
        }

        return new TodoItem(id, userId, text, createdAt);
    }

    private TodoItem() { }

    public Guid UserId { get; private set; }
    
    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    /// <summary>Marks item as finished.</summary>
    public Result MarkAsFinished()
    {
        if (IsFinished)
        {
            return TodoItemDomainErrors.AlreadyFinished(Text);
        }

        IsFinished = true;
        FinishedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
