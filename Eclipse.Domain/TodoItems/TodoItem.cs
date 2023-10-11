using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.TodoItems;

public class TodoItem : Entity
{
    [JsonConstructor]
    internal TodoItem(Guid id, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null)
        : base(id)
    { 
        Text = text;
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
        IsFinished = isFinished;
    }

    internal TodoItem(Guid id, Guid userId, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null) : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = createdAt;
        IsFinished = isFinished;
        FinishedAt = finishedAt;
    }

    private TodoItem() { }

    public Guid UserId { get; set; }
    
    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

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
