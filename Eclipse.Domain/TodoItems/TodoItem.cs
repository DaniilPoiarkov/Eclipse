using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.TodoItems;

public class TodoItem : Entity
{
    public TodoItem(Guid id, long telegramUserId, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null)
        : base(id)
    {
        TelegramUserId = telegramUserId;
        Text = text;
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
        IsFinished = isFinished;
    }

    private TodoItem() { }

    public long TelegramUserId { get; private set; }

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
