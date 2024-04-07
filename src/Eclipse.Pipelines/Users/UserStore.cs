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

    public async Task<Result> AddOrUpdateAsync(TelegramUser user, CancellationToken cancellationToken = default)
    {
        var cached = await _userCache.GetByChatIdAsync(user.Id, cancellationToken);

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
            Username = user.Username ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id
        };

        var creationResult = await _identityUserService.CreateAsync(createUserDto, cancellationToken);

        if (!creationResult.IsSuccess)
        {
            return creationResult.Error;
        }

        await _userCache.AddOrUpdateAsync(creationResult.Value, cancellationToken);
        return Result.Success();
    }

    public Task<IReadOnlyList<IdentityUserDto>> GetCachedUsersAsync(CancellationToken cancellationToken = default) => _userCache.GetAllAsync(cancellationToken);

    private async Task<Result> CheckAndUpdate(IdentityUserDto identityDto, TelegramUser telegramUser, CancellationToken cancellationToken)
    {
        if (HaveSameValues(identityDto, telegramUser))
        {
            await _userCache.AddOrUpdateAsync(identityDto, cancellationToken);
            return Result.Success();
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
            return result.Error;
        }

        await _userCache.AddOrUpdateAsync(result, cancellationToken);

        return Result.Success();

        static bool HaveSameValues(IdentityUserDto identityDto, TelegramUser telegramUser)
        {
            return identityDto.Name == telegramUser.Name
                && identityDto.Username == telegramUser.Username
                && identityDto.Surname == telegramUser.Surname;
        }
    }
}
