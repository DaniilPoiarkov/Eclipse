using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal interface IIdentityUserCache
{
    void AddOrUpdate(IdentityUserDto user);

    IReadOnlyList<IdentityUserDto> GetUsers();

    IdentityUserDto? GetByChatId(long chatId);

    IdentityUserDto? GetByUserId(Guid userId);
}
