using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Telegram.Commands;

using FluentValidation;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Commands;

internal sealed class CommandService : ICommandService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IValidator<CommandDto> _commandDtoValidator;

    private readonly IMapper<BotCommand, CommandDto> _mapper;

    public CommandService(
        ITelegramBotClient botClient,
        IValidator<CommandDto> commandDtoValidator,
        IMapper<BotCommand, CommandDto> mapper)
    {
        _botClient = botClient;
        _commandDtoValidator = commandDtoValidator;
        _mapper = mapper;
    }

    public async Task Add(CommandDto command, CancellationToken cancellationToken = default)
    {
        _commandDtoValidator.ValidateAndThrow(command);

        var commands = await GetMyCommands(cancellationToken: cancellationToken);

        var newCommands = new List<BotCommand>(commands.Length + 1);

        newCommands.AddRange(commands);
        newCommands.Add(new BotCommand()
        {
            Command = command.Command,
            Description = command.Description,
        });

        await SetCommands(newCommands, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<CommandDto>> GetList(CancellationToken cancellationToken = default)
    {
        var commands = await GetMyCommands(cancellationToken: cancellationToken);
        
        return commands.Select(_mapper.Map)
            .ToArray();
    }

    public async Task Remove(string command, CancellationToken cancellationToken = default)
    {
        var commands = await GetMyCommands(cancellationToken);
        var newCommands = commands.Where(c => !c.Command.Equals(command));

        await SetCommands(newCommands, cancellationToken: cancellationToken);
    }

    private async Task SetCommands(IEnumerable<BotCommand> commands, CancellationToken cancellationToken = default)
    {
        await _botClient.SetMyCommandsAsync(commands, BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);
    }

    private async Task<BotCommand[]> GetMyCommands(CancellationToken cancellationToken = default)
    {
        return await _botClient.GetMyCommandsAsync(
            BotCommandScope.AllPrivateChats(),
            cancellationToken: cancellationToken);
    }
}
