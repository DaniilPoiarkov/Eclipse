namespace Eclipse.Application.Contracts.IdentityUsers;

public interface IIdentityUserCache
{
    Task AddOrUpdateAsync(IdentityUserDto user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IdentityUserDto?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);

    Task<IdentityUserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
