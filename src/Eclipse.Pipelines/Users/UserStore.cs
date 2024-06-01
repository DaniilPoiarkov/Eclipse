using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Core.Models;

namespace Eclipse.Pipelines.Users;

internal sealed class UserStore : IUserStore
{
    private readonly IUserService _userService;

    private readonly IUserCache _userCache;

    public UserStore(IUserService userService, IUserCache userCache)
    {
        _userService = userService;
        _userCache = userCache;
    }

    public async Task<Result> AddOrUpdateAsync(TelegramUser user, CancellationToken cancellationToken = default)
    {
        var cached = await _userCache.GetByChatIdAsync(user.Id, cancellationToken);

        if (cached is not null)
        {
            return await CheckAndUpdate(cached, user, cancellationToken);
        }

        var userResult = await _userService.GetByChatIdAsync(user.Id, cancellationToken);

        if (userResult.IsSuccess)
        {
            return await CheckAndUpdate(userResult.Value, user, cancellationToken);
        }

        var createUserDto = new UserCreateDto
        {
            Name = user.Name,
            UserName = user.UserName ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id
        };

        var creationResult = await _userService.CreateAsync(createUserDto, cancellationToken);

        if (!creationResult.IsSuccess)
        {
            return creationResult.Error;
        }

        await _userCache.AddOrUpdateAsync(creationResult.Value, cancellationToken);
        return Result.Success();
    }

    public Task<IReadOnlyList<UserDto>> GetCachedUsersAsync(CancellationToken cancellationToken = default) => _userCache.GetAllAsync(cancellationToken);

    private async Task<Result> CheckAndUpdate(UserDto userDto, TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        if (HaveSameValues(userDto, telegramUser))
        {
            await _userCache.AddOrUpdateAsync(userDto, cancellationToken);
            return Result.Success();
        }

        var updateDto = new UserUpdateDto
        {
            Name = telegramUser.Name,
            UserName = telegramUser.UserName,
            Surname = telegramUser.Surname
        };

        var result = await _userService.UpdateAsync(userDto.Id, updateDto, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        await _userCache.AddOrUpdateAsync(result, cancellationToken);

        return Result.Success();

        static bool HaveSameValues(UserDto user, TelegramUser telegramUser)
        {
            return user.Name == telegramUser.Name
                && user.UserName == telegramUser.UserName
                && user.Surname == telegramUser.Surname;
        }
    }
}
