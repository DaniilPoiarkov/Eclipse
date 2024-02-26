using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Exceptions;
using Eclipse.Core.Models;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Pipelines.Users;

internal sealed class UserStore : IUserStore
{
    private readonly IIdentityUserService _identityUserService;

    private readonly IIdentityUserCache _userCache;

    public UserStore(IIdentityUserService identityUserService, IIdentityUserCache userCache)
    {
        _identityUserService = identityUserService;
        _userCache = userCache;
    }

    public async Task AddOrUpdate(TelegramUser user, CancellationToken cancellationToken = default)
    {
        var cached = _userCache.GetByChatId(user.Id);

        if (cached is not null)
        {
            await CheckAndUpdate(cached, user, cancellationToken);
            return;
        }

        var userResult = await _identityUserService.GetByChatIdAsync(user.Id, cancellationToken);

        if (userResult.IsSuccess)
        {
            await CheckAndUpdate(userResult.Value, user, cancellationToken);
            return;
        }

        var createUserDto = new IdentityUserCreateDto
        {
            Name = user.Name,
            Username = user.Username ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id
        };

        var creationResult = await _identityUserService.CreateAsync(createUserDto, cancellationToken);

        if (!creationResult.IsSuccess)
        {
            // TODO: Remove
            throw new EclipseValidationException("Create user validation");
        }

        _userCache.AddOrUpdate(creationResult.Value);
    }

    public IReadOnlyList<IdentityUserDto> GetCachedUsers() => _userCache.GetAll();

    private async Task CheckAndUpdate(IdentityUserDto identityDto, TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        if (HaveSameValues(identityDto, telegramUser))
        {
            _userCache.AddOrUpdate(identityDto);
            return;
        }

        var updateDto = new IdentityUserUpdateDto
        {
            Name = telegramUser.Name,
            Username = telegramUser.Username,
            Surname = telegramUser.Surname
        };

        var result = await _identityUserService.UpdateAsync(identityDto.Id, updateDto, cancellationToken);

        if (!result.IsSuccess)
        {
            // TODO: Remove
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        _userCache.AddOrUpdate(result);

        static bool HaveSameValues(IdentityUserDto identityDto, TelegramUser telegramUser)
        {
            return identityDto.Name == telegramUser.Name
                && identityDto.Username == telegramUser.Username
                && identityDto.Surname == telegramUser.Surname;
        }
    }
}
