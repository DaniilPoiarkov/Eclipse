using Eclipse.Application.Contracts.Reminders;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

[Route("Menu:Reminders:Add", "/reminders_add")]
internal sealed class AddReminderPipeline : RemindersPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IReminderService _reminderService;

    private static readonly string _pipelinePrefix = "Pipelines:Reminders";

    public AddReminderPipeline(ICacheService cacheService, IReminderService reminderService)
    {
        _cacheService = cacheService;
        _reminderService = reminderService;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(new ReplyKeyboardRemove(), Localizer[$"{_pipelinePrefix}:AskForText"]));
        RegisterStage(AskForTime);
        RegisterStage(SaveReminder);
    }

    private async Task<IResult> AskForTime(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            RegisterStage(AskForTime);
            RegisterStage(SaveReminder);

            return Text(Localizer[$"{_pipelinePrefix}:ValueCannotBeEmpty"]);
        }

        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            FinishPipeline();
            return Menu(RemindersMenuButtons, Localizer["Okay"]);
        }

        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays
        };

        await _cacheService.SetAsync(
            $"reminder-text-{context.ChatId}",
            context.Value,
            options,
            cancellationToken
        );

        return Menu(new ReplyKeyboardRemove(), Localizer[$"{_pipelinePrefix}:AskForTime"]);
    }

    private async Task<IResult> SaveReminder(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            return Menu(RemindersMenuButtons, Localizer["Okay"]);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            RegisterStage(SaveReminder);
            return Text(Localizer[$"{_pipelinePrefix}:CannotParseTime"]);
        }

        var chatId = context.ChatId;

        var text = await _cacheService.GetOrCreateAsync(
            $"reminder-text-{chatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (text.IsNullOrEmpty())
        {
            return Menu(RemindersMenuButtons, Localizer["Error"]);
        }

        var createModel = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = time
        };

        var result = await _reminderService.CreateAsync(chatId, createModel, cancellationToken);

        var message = result.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:Created"]
            : Localizer["Error"];

        return Menu(RemindersMenuButtons, message);
    }
}
