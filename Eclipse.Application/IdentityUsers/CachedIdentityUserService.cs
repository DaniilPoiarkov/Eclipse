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

    public async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserService.CreateAsync(createDto, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
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

    public async Task<IdentityUserDto> GetByChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var user = _userCache.GetByChatId(chatId);

        if (user is not null)
        {
            return user;
        }

        user = await _identityUserService.GetByChatIdAsync(chatId, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
    }

    public async Task<IdentityUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _userCache.GetById(id);

        if (user is not null)
        {
            return user;
        }

        user = await _identityUserService.GetByIdAsync(id, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
    }

    public async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        var user = await _identityUserService.UpdateAsync(id, updateDto, cancellationToken);

        _userCache.AddOrUpdate(user);

        return user;
    }
}
