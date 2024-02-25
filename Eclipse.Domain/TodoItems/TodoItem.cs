using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Entities;

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
