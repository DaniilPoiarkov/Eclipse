using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Core.Models;

namespace Eclipse.Application.IdentityUsers;

internal class CachedIdentityUserService : IIdentityUserService
{
    private readonly IIdentityUserCache _userCache;

    private readonly IIdentityUserService _identityUserService;

    public CachedIdentityUserService(IIdentityUserCache userCache, IIdentityUserService identityUserService)
    {
        _userCache = userCache;
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
            _userCache.AddOrUpdate(user);
        }

        return users;
    }

    public Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = _userCache.GetByChatId(chatId);

        if (user is not null)
        {
            return Task.FromResult(user);
        }

        return WithCachingAsync(() => _identityUserService.GetByChatIdAsync(chatId, cancellationToken));
    }

    public Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _userCache.GetById(id);

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

    private async Task<IdentityUserDto> WithCachingAsync(Func<Task<IdentityUserDto>> action)
    {
        var user = await action();

        _userCache.AddOrUpdate(user);

        return user;
    }
}
