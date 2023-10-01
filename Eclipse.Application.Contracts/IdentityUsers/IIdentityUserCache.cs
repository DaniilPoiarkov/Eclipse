namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserCache
{
    void EnsureAdded(IdentityUserDto user);

    IReadOnlyList<IdentityUserDto> GetUsers();
}
