﻿using Eclipse.Application.Contracts.Telegram.BotManagement;
using FluentValidation;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Application.Telegram.Commands;

internal class CommandService : ICommandService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IValidator<CommandDto> _commandDtoValidator;

    public CommandService(ITelegramBotClient botClient, IValidator<CommandDto> commandDtoValidator)
    {
        _botClient = botClient;
        _commandDtoValidator = commandDtoValidator;
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

        await _botClient.SetMyCommandsAsync(newCommands, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<CommandDto>> GetList(CancellationToken cancellationToken = default)
    {
        var commands = await GetMyCommands(cancellationToken: cancellationToken);

        return commands.Select(c => new CommandDto()
        {
            Command = c.Command,
            Description = c.Description,
        }).ToArray();
    }

    public async Task Remove(string command, CancellationToken cancellationToken = default)
    {
        var commands = await GetMyCommands(cancellationToken);
        var newCommands = commands.Where(c => !c.Command.Equals(command));
        await _botClient.SetMyCommandsAsync(newCommands, cancellationToken: cancellationToken);
    }

    private async Task<BotCommand[]> GetMyCommands(CancellationToken cancellationToken = default)
    {
        return await _botClient.GetMyCommandsAsync(cancellationToken: cancellationToken);
    }
}
