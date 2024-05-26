using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

[Serializable]
public sealed record NewUserJoinedDomainEvent : IDomainEvent
{
    public Guid UserId { get; }

    public string UserName { get; }

    public string Name { get; }

    public string Surname { get; }

    internal NewUserJoinedDomainEvent(Guid userId, string userName, string name, string surname)
    {
        UserId = userId;
        UserName = userName;
        Name = name;
        Surname = surname;
    }
}
