using Eclipse.Application.Contracts.Telegram;

using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram;

internal static class CommandExtensions
{
    public static CommandDto ToDto(this BotCommand command)
    {
        return new CommandDto(command.Command, command.Description);
    }
}
