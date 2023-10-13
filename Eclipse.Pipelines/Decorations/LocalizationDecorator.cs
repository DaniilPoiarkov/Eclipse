using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Decorations;

public class LocalizationDecorator : ICoreDecorator
{
    private readonly IdentityUserManager _userManager;

    private readonly ICacheService _cacheService;

    private readonly IEclipseLocalizer _localizer;

    public LocalizationDecorator(IdentityUserManager userManager, ICacheService cacheService, IEclipseLocalizer localizer)
    {
        _userManager = userManager;
        _cacheService = cacheService;
        _localizer = localizer;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        await CheckLocalizationAsync(context, cancellationToken);

        return await execution(context, cancellationToken);
    }

    private async Task CheckLocalizationAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var key = new CacheKey($"lang-{context.ChatId}");

        var culture = _cacheService.Get<string>(key);

        if (culture is not null)
        {
            _localizer.CheckCulture(context.ChatId);
            return;
        }

        var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is not null)
        {
            _cacheService.Set(key, user.Culture);
            _localizer.CheckCulture(user.ChatId);
        }
    }
}
