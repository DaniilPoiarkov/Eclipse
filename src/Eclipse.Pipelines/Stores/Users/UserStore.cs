using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Core.Context;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.Stores.Users;

internal sealed class UserStore : IUserStore
{
    private readonly IUserService _userService;

    public UserStore(IUserService userService)
    {
        _userService = userService;
    }

    public async Task CreateOrUpdateAsync(TelegramUser user, Update update, CancellationToken cancellationToken = default)
    {
        var botDisabled = update.MyChatMember?.NewChatMember.Status == ChatMemberStatus.Kicked;
        var result = await _userService.GetByChatIdAsync(user.Id, cancellationToken);

        if (!result.IsSuccess)
        {
            await Create(user, cancellationToken);
            return;
        }

        if (!ShouldUpdate(result.Value, user, botDisabled))
        {
            return;
        }

        await Update(user, botDisabled, result, cancellationToken);
    }

    private async Task Update(TelegramUser user, bool botDisabled, Result<UserDto> result, CancellationToken cancellationToken)
    {
        var model = new UserPartialUpdateDto
        {
            NameChanged = true,
            Name = user.Name,

            UserNameChanged = true,
            UserName = user.UserName,

            SurnameChanged = true,
            Surname = user.Surname,

            IsEnabledChanged = true,
            IsEnabled = !botDisabled
        };

        await _userService.UpdatePartialAsync(result.Value.Id, model, cancellationToken);
    }

    private async Task Create(TelegramUser user, CancellationToken cancellationToken)
    {
        var create = new UserCreateDto
        {
            Name = user.Name,
            UserName = user.UserName ?? string.Empty,
            Surname = user.Surname,
            ChatId = user.Id,
            NotificationsEnabled = true,
        };

        await _userService.CreateAsync(create, cancellationToken);
    }

    private static bool HaveSameValues(UserDto user, TelegramUser telegramUser)
    {
        return user.Name == telegramUser.Name
            && user.UserName == telegramUser.UserName
            && user.Surname == telegramUser.Surname;
    }

    private static bool ShouldUpdate(UserDto user, TelegramUser telegramUser, bool botDisabled)
    {
        return !HaveSameValues(user, telegramUser)
            || user.IsEnabled == botDisabled;
    }
}
