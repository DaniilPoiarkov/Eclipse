using Eclipse.Common.Caching;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

namespace Eclipse.Pipelines.Decorations;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly UserManager _userManager;

    private readonly ICacheService _cacheService;

    private readonly ICurrentCulture _currentCulture;

    public LocalizationDecorator(UserManager userManager, ICacheService cacheService, ICurrentCulture currentCulture)
    {
        _userManager = userManager;
        _cacheService = cacheService;
        _currentCulture = currentCulture;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"lang-{context.ChatId}");

        var culture = await _cacheService.GetAsync<string>(key, cancellationToken);

        if (culture is null)
        {
            var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

            if (user is not null)
            {
                await _cacheService.SetAsync(key, user.Culture, CacheConsts.ThreeDays, cancellationToken);
                culture = user.Culture;
            }
        }

        using var _ = _currentCulture.UsingCulture(culture ?? _currentCulture.Culture);

        return await execution(context, cancellationToken);
    }
}
