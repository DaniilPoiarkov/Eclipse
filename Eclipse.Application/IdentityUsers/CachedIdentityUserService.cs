using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal sealed class CachedIdentityUserService : IdentityUserCachingFixture, IIdentityUserService
{
    private readonly IIdentityUserService _identityUserService;

    public CachedIdentityUserService(IIdentityUserCache userCache, IIdentityUserService identityUserService) : base(userCache)
    {
        _identityUserService = identityUserService;
    }

    public Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.CreateAsync(createDto, cancellationToken));
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _identityUserService.GetAllAsync(cancellationToken);

        foreach (var user in users)
        {
            UserCache.AddOrUpdate(user);
        }

        return users;
    }

    public Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = UserCache.GetByChatId(chatId);

        if (user is not null)
        {
            return Task.FromResult(user);
        }

        return WithCachingAsync(() => _identityUserService.GetByChatIdAsync(chatId, cancellationToken));
    }

    public Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = UserCache.GetById(id);

        if (user is not null)
        {
            return Task.FromResult(user);
        }

        return WithCachingAsync(() => _identityUserService.GetByIdAsync(id, cancellationToken));
    }

    public Task<IdentityUserDto> SetUserGmtTimeAsync(Guid userId, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.SetUserGmtTimeAsync(userId, currentUserTime, cancellationToken));
    }

    public Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.UpdateAsync(id, updateDto, cancellationToken));
    }
}
