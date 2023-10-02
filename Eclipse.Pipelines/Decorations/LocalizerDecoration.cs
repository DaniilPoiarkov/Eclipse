using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Infrastructure.Cache;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Pipelines.Decorations;

public class LocalizerDecoration : ICoreDecorator
{
    //private readonly IdentityUserManager _userManager;

    //private readonly ICacheService _cacheService;

    //private readonly IEclipseLocalizer _localizer;

    //private readonly IServiceProvider _serviceProvider;

    //public LocalizerDecoration(IdentityUserManager userManager, ICacheService cacheService, IEclipseLocalizer localizer, IServiceProvider serviceProvider)
    //{
    //    _userManager = userManager;
    //    _cacheService = cacheService;
    //    _localizer = localizer;
    //    _serviceProvider = serviceProvider;
    //}

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"lang-{context.ChatId}");

        var cache = CachedServiceProvider.Services.GetRequiredService<ICacheService>();
        var localizer = CachedServiceProvider.Services.GetRequiredService<IEclipseLocalizer>();
        var manager = CachedServiceProvider.Services.GetRequiredService<IdentityUserManager>();

        var culture = cache.Get<string>(key);

        if (culture is not null)
        {
            return await execution(context, cancellationToken);
        }

        var user = await manager.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is not null)
        {
            cache.Set(key, user.Culture);
            localizer.CheckCulture(user.ChatId);
        }

        return await execution(context, cancellationToken);
    }
}
