using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily.Morning;

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
        var mood = context.Value switch
        {
            "👍" => new MoodAnswer(MoodState.Good, "Pipelines:Morning:GoodMood"),
            "👎" => new MoodAnswer(MoodState.Bad, "Pipelines:Morning:BadMood"),
            _ => new MoodAnswer(null, "Pipelines:Morning:NotDefined")
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        if (mood.State is null)
        {
            return EditedOrDefaultResult(message, Text(Localizer[mood.Message]));
        }

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);
        
        var model = new CreateMoodRecordDto
        {
            State = mood.State.Value
        };

        await _service.CreateAsync(user.Value.Id, model, cancellationToken);

        return EditedOrDefaultResult(message, Text(Localizer[mood.Message]));
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
