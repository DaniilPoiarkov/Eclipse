using Eclipse.Infrastructure.Telegram;

using FluentValidation;

using Telegram.Bot;

namespace Eclipse.Infrastructure.Internals.Telegram;

internal class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IValidator<SendMessageModel> _validator;

    public TelegramService(ITelegramBotClient botClient, IValidator<SendMessageModel> validator)
    {
        _botClient = botClient;
        _validator = validator;
    }

    public async Task Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        _validator.ValidateAndThrow(message);

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);
    }
}
