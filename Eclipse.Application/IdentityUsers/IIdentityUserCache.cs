using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal interface IIdentityUserCache
{
    void AddOrUpdate(IdentityUserDto user);

    IReadOnlyList<IdentityUserDto> GetAll();

    IdentityUserDto? GetByChatId(long chatId);

    IdentityUserDto? GetById(Guid userId);
}
