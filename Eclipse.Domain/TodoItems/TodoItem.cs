using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.TodoItems;

using Newtonsoft.Json;

namespace Eclipse.Domain.TodoItems;

public class TodoItem : Entity
{
    [JsonConstructor]
    public TodoItem(Guid id, long telegramUserId, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null)
        : base(id)
    {
        TelegramUserId = telegramUserId;
        Text = text;
        CreatedAt = createdAt;
        FinishedAt = finishedAt;
        IsFinished = isFinished;
    }

    public TodoItem(Guid id, IdentityUser user, string text, DateTime createdAt, bool isFinished = false, DateTime? finishedAt = null) : base(id)
    {
        UserId = user.Id;
        TelegramUserId = user.ChatId;
        Text = text;
        CreatedAt = createdAt;
        IsFinished = isFinished;
        FinishedAt = finishedAt;
    }

    private TodoItem() { }

    public Guid UserId { get; set; }

    public long TelegramUserId { get; private set; }

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
