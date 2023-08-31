﻿using Eclipse.WebAPI.Services.Cache;
using Eclipse.WebAPI.Services.UserStores;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.WebAPI.Services.TelegramServices.Implementations;

public class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ILogger<TelegramUpdateHandler> _logger;

    private readonly IUserStore _userStore;

    public TelegramUpdateHandler(ILogger<TelegramUpdateHandler> logger, IUserStore userStore)
    {
        _logger = logger;
        _userStore = userStore;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("Telegram error: {ex}", exception.Message);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
        {
            _logger.LogInformation("Update is not type of message");
            return;
        }

        _logger.LogInformation("Recieved message from {chatId} (chatId)", update.Message!.Chat.Id);
        
        await botClient.SendTextMessageAsync(
            update.Message!.Chat.Id,
            "Hello! I'm Eclipse. Right now I'm having a rest, so see you later",
            cancellationToken: cancellationToken);

        _userStore.AddUser(new TelegramUser(update));
    }
}
