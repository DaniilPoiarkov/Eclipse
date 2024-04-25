using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Core.Models;

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

    public async Task<Result> AddOrUpdate(TelegramUser user, CancellationToken cancellationToken = default)
    {
        var cached = _userCache.GetByChatId(user.Id);

        if (cached is not null)
        {
            return await CheckAndUpdate(cached, user, cancellationToken);
        }

        var userResult = await _identityUserService.GetByChatIdAsync(user.Id, cancellationToken);

        if (userResult.IsSuccess)
        {
            return await CheckAndUpdate(userResult.Value, user, cancellationToken);
        }

        var createUserDto = new IdentityUserCreateDto
        {
            Name = user.Name,
            UserName = user.UserName ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id
        };

        var creationResult = await _identityUserService.CreateAsync(createUserDto, cancellationToken);

        if (!creationResult.IsSuccess)
        {
            return creationResult.Error;
        }

        _userCache.AddOrUpdate(creationResult.Value);
        return Result.Success();
    }

    public IReadOnlyList<IdentityUserDto> GetCachedUsers() => _userCache.GetAll();

    private async Task<Result> CheckAndUpdate(IdentityUserDto identityDto, TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        if (HaveSameValues(identityDto, telegramUser))
        {
            _userCache.AddOrUpdate(identityDto);
            return Result.Success();
        }

        var updateDto = new IdentityUserUpdateDto
        {
            Name = telegramUser.Name,
            UserName = telegramUser.UserName,
            Surname = telegramUser.Surname
        };

        var result = await _identityUserService.UpdateAsync(identityDto.Id, updateDto, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        _userCache.AddOrUpdate(result);

        return Result.Success();

        static bool HaveSameValues(IdentityUserDto identityDto, TelegramUser telegramUser)
        {
            return identityDto.Name == telegramUser.Name
                && identityDto.UserName == telegramUser.UserName
                && identityDto.Surname == telegramUser.Surname;
        }
    }
}
