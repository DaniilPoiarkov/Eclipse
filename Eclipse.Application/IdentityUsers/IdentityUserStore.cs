using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Core.Models;
using Eclipse.Domain.Shared.IdentityUsers;

namespace Eclipse.Application.IdentityUsers;

internal class IdentityUserStore : IIdentityUserStore
{
    private readonly IIdentityUserCache _userCache;

    private readonly IIdentityUserService _identityUserService;

    public IdentityUserStore(IIdentityUserCache userCache, IIdentityUserService identityUserService)
    {
        _userCache = userCache;
        _identityUserService = identityUserService;
    }

    public async Task EnsureAdded(TelegramUser user, CancellationToken cancellationToken = default)
    {
        var cached = _userCache.GetUsers().FirstOrDefault(u => u.ChatId == user.Id);

        if (cached is not null)
        {
            await CheckAndUpdate(cached, user, cancellationToken);
            return;
        }

        try
        {
            var entity = await _identityUserService.GetByChatIdAsync(user.Id, cancellationToken);
            await CheckAndUpdate(entity, user, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            var createUserDto = new IdentityUserCreateDto
            {
                Name = user.Name,
                Username = user.Username ?? string.Empty,
                ChatId = user.Id
            };

            var created = await _identityUserService.CreateAsync(createUserDto, cancellationToken);

            _userCache.EnsureAdded(created!);
        }
    }

    public async Task<IReadOnlyList<IdentityUserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _identityUserService.GetAllAsync(cancellationToken);
    }

    private async Task CheckAndUpdate(IdentityUserDto identityDto, TelegramUser user, CancellationToken cancellationToken)
    {
        if (identityDto.Name == user.Name
            && identityDto.Username == user.Username
            && identityDto.Surname == user.Surname)
        {
            _userCache.EnsureAdded(identityDto);
            return;
        }

        var updateDto = new IdentityUserUpdateDto
        {
            Name = user.Name,
            Username = user.Username,
            Surname = user.Surname
        };

        var updated = await _identityUserService.UpdateAsync(identityDto.Id, updateDto, cancellationToken);
        _userCache.EnsureAdded(updated!);
    }
}
