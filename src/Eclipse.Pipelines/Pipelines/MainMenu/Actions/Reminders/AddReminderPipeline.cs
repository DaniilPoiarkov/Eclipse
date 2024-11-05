using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Caching;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

[Route("Menu:Reminders:Add", "/reminders_add")]
internal sealed class AddReminderPipeline : RemindersPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IUserService _userService;

    private readonly IReminderService _reminderService;

    private static readonly string _pipelinePrefix = "Pipelines:Reminders";

    public AddReminderPipeline(ICacheService cacheService, IUserService userService, IReminderService reminderService)
    {
        _cacheService = cacheService;
        _userService = userService;
        _reminderService = reminderService;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer[$"{_pipelinePrefix}:AskForText"]));
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

        await _cacheService.SetAsync(new CacheKey($"reminder-text-{context.ChatId}"), context.Value, CacheConsts.ThreeDays, cancellationToken);

        return Text(Localizer[$"{_pipelinePrefix}:AskForTime"]);
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

        var userResult = await _userService.GetByChatIdAsync(chatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Menu(RemindersMenuButtons, Localizer["Error"]);
        }

        var text = await _cacheService.GetAsync<string>(new CacheKey($"reminder-text-{chatId}"), cancellationToken);

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
