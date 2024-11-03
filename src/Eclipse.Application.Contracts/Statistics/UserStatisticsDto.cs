using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.Statistics;

[Serializable]
public sealed class UserStatisticsDto : EntityDto
{
    public Guid UserId { get; init; }

    public int TodoItemsFinished { get; init; }

    public int RemindersReceived { get; init; }
}
