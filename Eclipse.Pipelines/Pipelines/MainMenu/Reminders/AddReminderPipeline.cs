using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

[Route("Menu:Reminders:Add", "/reminders_add")]
public class AddReminderPipeline : RemindersPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IIdentityUserService _identityUserService;

    private readonly IReminderService _reminderService;

    public AddReminderPipeline(ICacheService cacheService, IIdentityUserService identityUserService, IReminderService reminderService)
    {
        _cacheService = cacheService;
        _identityUserService = identityUserService;
        _reminderService = reminderService;
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer["Pipelines:Reminders:AskForText"]));
        RegisterStage(AskForTime);
        RegisterStage(SaveReminder);
    }

    private IResult AskForTime(MessageContext context)
    {
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders:ValueCannotBeEmpty"]);
        }

        _cacheService.Set(new CacheKey($"reminder-text-{context.ChatId}"), context.Value);

        return Text(Localizer["Pipelines:Reminders:AskForTime"]);
    }

    private async Task<IResult> SaveReminder(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (!TryParseTime(context.Value, out var time))
        {
            return Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders:CannotParseTime"]);
        }

        var chatId = context.ChatId;

        var user = await _identityUserService.GetByChatIdAsync(chatId, cancellationToken);

        var text = _cacheService.Get<string>(new CacheKey($"reminder-text-{chatId}"))!;

        var reminderCreateDto = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = time
        };
        
        await _reminderService.CreateReminderAsync(user.Id, reminderCreateDto, cancellationToken);

        return Menu(RemindersMenuButtons, Localizer["Pipelines:Reminders:Created"]);
    }
}
