using Eclipse.Application.Reminders.Sendings;
using Eclipse.Localization.Culture;

using Microsoft.Extensions.Localization;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Reminders;

internal sealed class ReminderSender : IReminderSender
{
    private readonly ITelegramBotClient _client;

    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<ReminderSender> _localizer;

    public ReminderSender(
        ITelegramBotClient client,
        ICurrentCulture currentCulture,
        IStringLocalizer<ReminderSender> localizer)
    {
        _client = client;
        _currentCulture = currentCulture;
        _localizer = localizer;
    }

    public async Task Send(ReminderArguments arguments, CancellationToken cancellationToken = default)
    {
        using var _ = _currentCulture.UsingCulture(arguments.Culture);

        if (!arguments.RelatedItemId.HasValue)
        {
            await _client.SendMessage(
                arguments.ChatId,
                $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{arguments.Text}",
                cancellationToken: cancellationToken
            );

            return;
        }

        // TODO: Start pipeline.
        var message = $"{_localizer["Jobs:SendReminders:Message"]}\n\r\n\r{arguments.Text}";

        // TODO: Localize button texts, start according pipeline
        List<InlineKeyboardButton> menu = arguments.RelatedItemId.HasValue
            ? [InlineKeyboardButton.WithCallbackData("Finish", arguments.RelatedItemId.Value.ToString()), InlineKeyboardButton.WithCallbackData("Reschedule", "reschedule")]
            : [];

        await _client.SendMessage(arguments.ChatId, message, replyMarkup: menu, cancellationToken: cancellationToken);
    }
}
