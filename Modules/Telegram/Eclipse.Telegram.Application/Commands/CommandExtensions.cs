using Eclipse.Telegram.Application.Contracts.Commands;

using Telegram.Bot.Types;

namespace Eclipse.Telegram.Application.Commands;

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
