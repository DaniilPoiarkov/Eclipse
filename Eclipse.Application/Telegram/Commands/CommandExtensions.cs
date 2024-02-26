using Eclipse.Application.Contracts.Telegram.Commands;

using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Commands;

internal static class CommandExtensions
{
    public static CommandDto ToDto(this BotCommand command)
    {
        return new CommandDto
        {
            Command = command.Command,
            Description = command.Description,
        };
    }
}
