using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;

namespace Eclipse.Application.IdentityUsers;

internal sealed class CachedIdentityUserService : IdentityUserCachingFixture, IIdentityUserService
{
    private readonly IIdentityUserService _identityUserService;

    public CachedIdentityUserService(IIdentityUserCache userCache, IIdentityUserService identityUserService) : base(userCache)
    {
        _identityUserService = identityUserService;
    }

    public Task<Result<IdentityUserDto>> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
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

    public Task<Result<IdentityUserDto>> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = UserCache.GetByChatId(chatId);

        if (user is not null)
        {
            return Task.FromResult(
                Result<IdentityUserDto>.Success(user)
            );
        }

        return WithCachingAsync(() => _identityUserService.GetByChatIdAsync(chatId, cancellationToken));
    }

    public Task<Result<IdentityUserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = UserCache.GetById(id);

        if (user is not null)
        {
            return Task.FromResult(
                Result<IdentityUserDto>.Success(user)
            );
        }

        return WithCachingAsync(() => _identityUserService.GetByIdAsync(id, cancellationToken));
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetFilteredListAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var users = await _identityUserService.GetFilteredListAsync(request, cancellationToken);

        foreach(var user in users)
        {
            UserCache.AddOrUpdate(user);
        }

        return users;
    }

    public Task<Result<IdentityUserDto>> SetUserGmtTimeAsync(Guid userId, TimeOnly currentUserTime, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.SetUserGmtTimeAsync(userId, currentUserTime, cancellationToken));
    }

    public Task<Result<IdentityUserDto>> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        return WithCachingAsync(() => _identityUserService.UpdateAsync(id, updateDto, cancellationToken));
    }
}
