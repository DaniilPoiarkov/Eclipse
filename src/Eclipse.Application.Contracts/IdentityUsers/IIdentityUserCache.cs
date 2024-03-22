namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserCache
{
    void AddOrUpdate(IdentityUserDto user);

    IReadOnlyList<IdentityUserDto> GetAll();

    IdentityUserDto? GetByChatId(long chatId);

    IdentityUserDto? GetById(Guid userId);
}
