using Eclipse.Domain.Base;

namespace Eclipse.Domain.Chronicles;

public class ChronicleItem : Entity
{
    public ChronicleItem(Guid id, Guid userId, string text)
        : base(id)
    {
        UserId = userId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public ChronicleItem() { }

    public Guid UserId { get; private set; }

    public string Text { get; private set; } = string.Empty;

    public bool IsFinished { get; private set; } = false;

    public DateTime CreatedAt { get; private set; }

    public DateTime? FinishedAt { get; private set; }


    public void MarkAsFinished()
    {
        IsFinished = true;
        FinishedAt = DateTime.UtcNow;
    }
}
