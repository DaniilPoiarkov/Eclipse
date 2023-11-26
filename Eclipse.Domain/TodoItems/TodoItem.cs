using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.TodoItems;

public sealed class TodoItem : Entity
{
    [JsonConstructor]
    internal TodoItem(Guid id, Guid userId, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null) : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = createdAt;
        IsFinished = isFinished;
        FinishedAt = finishedAt;
    }

    private TodoItem() { }

    public Guid UserId { get; private set; }
    
    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    /// <summary>Marks item as finished.</summary>
    /// <exception cref="TodoItemAlreadyFinishedException">If item already finished</exception>
    public void MarkAsFinished()
    {
        if (IsFinished)
        {
            throw new TodoItemAlreadyFinishedException(Text);
        }

        IsFinished = true;
        FinishedAt = DateTime.UtcNow;
    }
}
