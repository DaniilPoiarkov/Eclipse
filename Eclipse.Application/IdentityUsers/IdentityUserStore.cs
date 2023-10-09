using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Core.Models;

namespace Eclipse.Application.IdentityUsers;

internal class IdentityUserStore : IIdentityUserStore
{
    private readonly IIdentityUserService _identityUserService;

    private readonly IIdentityUserCache _userCache;

    public IdentityUserStore(IIdentityUserService identityUserService, IIdentityUserCache userCache)
    {
        _identityUserService = identityUserService;
        _userCache = userCache;
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

            var identity = await _identityUserService.CreateAsync(createUserDto, cancellationToken);
            _userCache.AddOrUpdate(identity);
        }
    }

    public IReadOnlyList<IdentityUserDto> GetCachedUsers() => _userCache.GetUsers();

    private async Task CheckAndUpdate(IdentityUserDto identityDto, TelegramUser user, CancellationToken cancellationToken)
    {
        if (identityDto.Name == user.Name
            && identityDto.Username == user.Username
            && identityDto.Surname == user.Surname)
        {
            _userCache.AddOrUpdate(identityDto);
            return;
        }

        var updateDto = new IdentityUserUpdateDto
        {
            Name = user.Name,
            Username = user.Username,
            Surname = user.Surname
        };

        await _identityUserService.UpdateAsync(identityDto.Id, updateDto, cancellationToken);
    }
}
