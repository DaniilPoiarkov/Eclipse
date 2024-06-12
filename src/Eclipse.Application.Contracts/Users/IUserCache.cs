namespace Eclipse.Application.Contracts.Users;

public interface IUserCache
{
    Task AddOrUpdateAsync(UserDto user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task InvalidateAllKeyAsync(CancellationToken cancellationToken = default);

    Task<UserDto?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
