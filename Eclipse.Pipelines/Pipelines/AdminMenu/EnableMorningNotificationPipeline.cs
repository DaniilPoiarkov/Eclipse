using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;
using Eclipse.Infrastructure.Quartz;
using Eclipse.Pipelines.Jobs.Morning;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:AdminMenu:Morning", "/admin_jobs_morning")]
internal class EnableMorningNotificationPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IEclipseScheduler _scheduler;

    public EnableMorningNotificationPipeline(ICacheService cacheService, IEclipseScheduler scheduler)
    {
        _cacheService = cacheService;
        _scheduler = scheduler;
    }

    protected override void Initialize()
    {
        RegisterStage(EnableMorningNotifications);
    }

    private async Task<IResult> EnableMorningNotifications(MessageContext context)
    {
        _cacheService.Set(new CacheKey($"notifications-enabled-{context.ChatId}"), true);
        var config = new MorningJobConfiguration();
        await _scheduler.ScheduleJob(config);
        
        return Text(Localizer["Pipelines:AdminMenu:MorningNotificationEnabled"]);
    }
}
