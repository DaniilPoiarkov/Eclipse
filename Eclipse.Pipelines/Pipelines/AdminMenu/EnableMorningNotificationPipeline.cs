using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("Menu:AdminMenu:Morning", "/admin_jobs_morning")]
internal class EnableMorningNotificationPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    public EnableMorningNotificationPipeline(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(EnableMorningNotifications);
    }

    private IResult EnableMorningNotifications(MessageContext context)
    {
        var key = new CacheKey($"notifications-enabled-{context.ChatId}");

        if (_cacheService.Get<bool>(key))
        {
            return Text(Localizer["Pipelines:AdminMenu:MorningNotificationAlreadyEnabled"]);
        }

        _cacheService.Set(key, true);
        
        return Text(Localizer["Pipelines:AdminMenu:MorningNotificationEnabled"]);
    }
}
