using Eclipse.Domain.Base;

namespace Eclipse.Domain.TodoItems;

public class TodoItem : Entity
{
    public TodoItem(Guid id, long userId, string text)
        : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    private TodoItem() { }

    public long UserId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }

    public void MarkAsFinished()
    {
        if (IsFinished)
        {
            throw new TodoItemAlreadyFinishedException(this);
        }

        IsFinished = true;
        FinishedAt = DateTime.UtcNow;
    }
}
