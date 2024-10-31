using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Statistics;

public sealed class UserStatistics : Entity
{
    public Guid UserId { get; private set; }

    public int TodoItemsFinished { get; private set; }
    public int RemindersRecieved { get; private set; }

    private UserStatistics() { }

    public UserStatistics(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }

    internal void ReminderRecieved(int count)
    {
        RemindersRecieved += count;
    }

    internal void TodoItemFinished()
    {
        TodoItemsFinished++;
    }
}
