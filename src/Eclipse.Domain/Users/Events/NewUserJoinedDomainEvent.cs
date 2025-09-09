using Eclipse.Common.Events;
using Eclipse.Domain.Shared.Users;

using Newtonsoft.Json;

namespace Eclipse.Domain.Users.Events;

[Serializable]
public sealed record NewUserJoinedDomainEvent : IDomainEvent, IHasUserId
{
    public Guid UserId { get; }

    public string UserName { get; }

    public string Name { get; }

    public string Surname { get; }

    [JsonConstructor]
    internal NewUserJoinedDomainEvent(Guid userId, string userName, string name, string surname)
    {
        UserId = userId;
        UserName = userName;
        Name = name;
        Surname = surname;
    }
}
