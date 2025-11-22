using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Shared.MoodRecords;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily.MoodRecords;

[Route("", "/href_mood_records_add")]
public sealed class AddMoodRecordPipeline : EclipsePipelineBase
{
    private readonly IMessageStore _messageStore;

    private readonly IMoodRecordsService _service;

    private readonly IUserService _userService;

    public AddMoodRecordPipeline(IMessageStore messageStore, IMoodRecordsService service, IUserService userService)
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
        var buttons = new InlineKeyboardButton[][]
        {
            [
                InlineKeyboardButton.WithCallbackData("1️⃣"),
                InlineKeyboardButton.WithCallbackData("2️⃣"),
                InlineKeyboardButton.WithCallbackData("3️⃣"),
                InlineKeyboardButton.WithCallbackData("4️⃣"),
                InlineKeyboardButton.WithCallbackData("5️⃣")
            ],
            [
                InlineKeyboardButton.WithCallbackData("6️⃣"),
                InlineKeyboardButton.WithCallbackData("7️⃣"),
                InlineKeyboardButton.WithCallbackData("8️⃣"),
                InlineKeyboardButton.WithCallbackData("9️⃣"),
                InlineKeyboardButton.WithCallbackData("🔟")
            ]
        };

        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        return EditedOrDefaultResult(message, Menu(buttons, Localizer["Pipelines:MoodRecords:Add:AskMood"]));
    }

    private async Task<IResult> HandleChoice(MessageContext context, CancellationToken cancellationToken = default)
    {
        var mood = context.Value switch
        {
            "🔟" => new MoodAnswer(MoodState.Amazing, "Pipelines:MoodRecords:Add:GoodMood"),
            "9️⃣" => new MoodAnswer(MoodState.Excellent, "Pipelines:MoodRecords:Add:GoodMood"),
            "8️⃣" => new MoodAnswer(MoodState.VeryGood, "Pipelines:MoodRecords:Add:GoodMood"),
            "7️⃣" => new MoodAnswer(MoodState.Good, "Pipelines:MoodRecords:Add:GoodMood"),
            "6️⃣" => new MoodAnswer(MoodState.Fine, "Pipelines:MoodRecords:Add:BadMood"),
            "5️⃣" => new MoodAnswer(MoodState.Neutral, "Pipelines:MoodRecords:Add:BadMood"),
            "4️⃣" => new MoodAnswer(MoodState.Poor, "Pipelines:MoodRecords:Add:BadMood"),
            "3️⃣" => new MoodAnswer(MoodState.Bad, "Pipelines:MoodRecords:Add:BadMood"),
            "2️⃣" => new MoodAnswer(MoodState.VeryBad, "Pipelines:MoodRecords:Add:BadMood"),
            "1️⃣" => new MoodAnswer(MoodState.Terrible, "Pipelines:MoodRecords:Add:BadMood"),
            _ => new MoodAnswer(null, "Pipelines:MoodRecords:Add:NotDefined")
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

        await _service.CreateOrUpdateAsync(user.Value.Id, model, cancellationToken);

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
