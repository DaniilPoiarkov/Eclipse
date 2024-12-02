using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Statistics;

public sealed class UserStatistics : Entity
{
    public Guid UserId { get; private set; }

    public int TodoItemsFinished { get; private set; }
    public int RemindersReceived { get; private set; }

    private UserStatistics() { }

    public UserStatistics(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }

    public void ReminderReceived()
    {
        RemindersReceived++;
    }

    public void TodoItemFinished()
    {
        TodoItemsFinished++;
    }
}
