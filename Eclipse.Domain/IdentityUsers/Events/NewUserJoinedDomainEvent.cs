using Eclipse.Domain.Shared.Events;

namespace Eclipse.Domain.IdentityUsers.Events;

public sealed class NewUserJoinedDomainEvent : DomainEvent
{
    public Guid UserId { get; }

    public string UserName { get; }

    public string Name { get; }

    public string Surname { get; }

    public NewUserJoinedDomainEvent(Guid userId, string userName, string name, string surname)
    {
        UserId = userId;
        UserName = userName;
        Name = name;
        Surname = surname;
    }
}
