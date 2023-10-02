using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Decorations;

public class LocalizerDecoration : ICoreDecorator
{
    private readonly IdentityUserManager _userManager;

    private readonly ICacheService _cacheService;

    private readonly IEclipseLocalizer _localizer;

    public LocalizerDecoration(IdentityUserManager userManager, ICacheService cacheService, IEclipseLocalizer localizer)
    {
        _userManager = userManager;
        _cacheService = cacheService;
        _localizer = localizer;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"lang-{context.ChatId}");

        var culture = _cacheService.Get<string>(key);

        if (culture is not null)
        {
            return await execution(context, cancellationToken);
        }

        var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is not null)
        {
            _cacheService.Set(key, user.Culture);
            _localizer.CheckCulture(user.ChatId);
        }

        return await execution(context, cancellationToken);
    }
}
