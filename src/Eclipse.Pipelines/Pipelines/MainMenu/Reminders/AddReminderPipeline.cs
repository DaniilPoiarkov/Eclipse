using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Localizations;
using Eclipse.Common.Cache;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

[Route("Menu:Reminders:Add", "/reminders_add")]
public sealed class AddReminderPipeline : RemindersPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IIdentityUserService _identityUserService;

    private readonly IReminderService _reminderService;

    private static readonly string _pipelinePrefix = "Pipelines:Reminders";

    public AddReminderPipeline(ICacheService cacheService, IIdentityUserService identityUserService, IReminderService reminderService)
    {
        _cacheService = cacheService;
        _identityUserService = identityUserService;
        _reminderService = reminderService;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer[$"{_pipelinePrefix}:AskForText"]));
        RegisterStage(AskForTime);
        RegisterStage(SaveReminder);
    }

    private IResult AskForTime(MessageContext context)
    {
        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(RemindersMenuButtons, Localizer[$"{_pipelinePrefix}:ValueCannotBeEmpty"]);
        }

        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            FinishPipeline();
            return Menu(RemindersMenuButtons, Localizer["Okay"]);
        }

        _cacheService.Set(new CacheKey($"reminder-text-{context.ChatId}"), context.Value);

        return Text(Localizer[$"{_pipelinePrefix}:AskForTime"]);
    }

    private async Task<IResult> SaveReminder(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            return Menu(RemindersMenuButtons, Localizer[$"{_pipelinePrefix}:CannotParseTime"]);
        }

        var chatId = context.ChatId;

        var userResult = await _identityUserService.GetByChatIdAsync(chatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return Menu(RemindersMenuButtons, Localizer.LocalizeError(userResult.Error));
        }

        var text = _cacheService.Get<string>(new CacheKey($"reminder-text-{chatId}"))!;

        var reminderCreateDto = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = time.Add(userResult.Value.Gmt * -1)
        };

        var result = await _reminderService.CreateReminderAsync(userResult.Value.Id, reminderCreateDto, cancellationToken);

        var message = result.IsSuccess
            ? Localizer[$"{_pipelinePrefix}:Created"]
            : Localizer.LocalizeError(result.Error);

        return Menu(RemindersMenuButtons, message);
    }
}
