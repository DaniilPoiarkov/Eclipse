using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Common.Results;

using FluentValidation;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Commands;

internal sealed class CommandService : ICommandService
{
    private readonly ITelegramBotClient _botClient;

    private static readonly string _descriptionPrefix = "BotCommand";

    public CommandService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Result> Add(AddCommandRequest request, CancellationToken cancellationToken = default)
    {
        var result = ValidateCommandCreateModel(request);

        if (!result.IsSuccess)
        {
            return result.Error;
        }

        var commands = await GetMyCommands(cancellationToken: cancellationToken);

        var newCommands = new List<BotCommand>(commands.Length + 1);

        newCommands.AddRange(commands);
        newCommands.Add(new BotCommand()
        {
            Command = request.Command!.ToLowerInvariant(),
            Description = request.Description!,
        });

        await SetCommands(newCommands, cancellationToken: cancellationToken);

        return Result.Success();
    }

    private static Result ValidateCommandCreateModel(AddCommandRequest command)
    {
        if (command.Command is not { Length: >= CommandConstants.CommandMinLength and <= CommandConstants.CommandMaxLength })
        {
            return Error.Validation("Command.Add", $"{_descriptionPrefix}:{(command.Command.IsNullOrEmpty() ? "CommandMinLength" : "CommandMaxLength")}");
        }

        if (command.Description is not { Length: >= CommandConstants.DescriptionMinLength and <= CommandConstants.DescriptionMaxLength })
        {
            return Error.Validation("Command.Add", $"{_descriptionPrefix}:{(command.Description?.Length < CommandConstants.DescriptionMinLength ? "DescriptionMinLength" : "DescriptionMaxLength")}");
        }

        return Result.Success();
    }

    public async Task<IReadOnlyList<CommandDto>> GetList(CancellationToken cancellationToken = default)
    {
        var commands = await GetMyCommands(cancellationToken: cancellationToken);
        
        return commands.Select(c => c.ToDto())
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
