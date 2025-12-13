namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class GetUsersRequest
{
    public string? Name { get; init; }
    public string? UserName { get; init; }
    public bool NotificationsEnabled { get; init; }
    public bool OnlyActive { get; init; }
    public IEnumerable<Guid> UserIds { get; init; } = [];
}
