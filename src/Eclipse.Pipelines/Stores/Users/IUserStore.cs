using Eclipse.Core.Context;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Users;

internal interface IUserStore
{
    Task CreateOrUpdateAsync(TelegramUser telegramUser, Update update, CancellationToken cancellationToken = default);
}
