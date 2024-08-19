using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily;

[Route("", "/daily_morning")]
public sealed class MorningPipeline : EclipsePipelineBase
{
    private readonly IMessageStore _messageStore;

    private readonly IMoodRecordsService _service;

    private readonly IUserService _userService;

    public MorningPipeline(IMessageStore messageStore, IMoodRecordsService service, IUserService userService)
    {
        _messageStore = messageStore;
        _service = service;
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(AskMood);
        RegisterStage(HandleChoice);
    }

    private async Task<IResult> AskMood(MessageContext context, CancellationToken cancellationToken = default)
    {
        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👍"),
            InlineKeyboardButton.WithCallbackData("👎"),
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Menu(buttons, Localizer["Pipelines:Morning:AskMood"]));
    }

    private async Task<IResult> HandleChoice(MessageContext context, CancellationToken cancellationToken = default)
    {
        var text = context.Value switch
        {
            "👍" => "Pipelines:Morning:GoodMood",
            "👎" => "Pipelines:Morning:BadMood",
            _ => "Pipelines:Morning:NotDefined"
        };

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        await _service.CreateAsync(user.Value.Id, true, cancellationToken);

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Text(Localizer[text]));
    }

    private static IResult EditedOrDefaultResult(Message? message, IResult @default)
    {
        return message is null || message.ReplyMarkup is null
            ? @default
            : Multiple(
                Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
                @default
            );
    }
}
