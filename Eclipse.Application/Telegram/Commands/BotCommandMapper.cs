using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Telegram.Commands;

using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Commands;

public sealed class BotCommandMapper : IMapper<BotCommand, CommandDto>
{
    public CommandDto Map(BotCommand value)
    {
        return new CommandDto
        {
            Command = value.Command,
            Description = value.Description,
        };
    }
}
