using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Common.Cache;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;

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
        if (string.IsNullOrEmpty(context.Value))
        {
            FinishPipeline();
            return Menu(RemindersMenuButtons, Localizer[$"{_pipelinePrefix}:ValueCannotBeEmpty"]);
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

        var result = await _identityUserService.GetByChatIdAsync(chatId, cancellationToken);

        if (!result.IsSuccess)
        {
            // TODO: Remove
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        var text = _cacheService.Get<string>(new CacheKey($"reminder-text-{chatId}"))!;

        var reminderCreateDto = new ReminderCreateDto
        {
            Text = text,
            NotifyAt = time.Add(result.Value.Gmt * -1)
        };
        
        await _reminderService.CreateReminderAsync(result.Value.Id, reminderCreateDto, cancellationToken);

        return Menu(RemindersMenuButtons, Localizer[$"{_pipelinePrefix}:Created"]);
    }
}
