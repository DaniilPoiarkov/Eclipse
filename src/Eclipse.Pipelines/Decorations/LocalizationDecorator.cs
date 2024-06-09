using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Common.Cache;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;

namespace Eclipse.Pipelines.Decorations;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly UserManager _userManager;

    private readonly ICacheService _cacheService;

    private readonly IEclipseLocalizer _localizer;

    public LocalizationDecorator(UserManager userManager, ICacheService cacheService, IEclipseLocalizer localizer)
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

        var culture = await _cacheService.GetAsync<string>(key, cancellationToken);

        if (culture is not null)
        {
            await _localizer.ResetCultureForUserWithChatIdAsync(context.ChatId, cancellationToken);
            return;
        }

        var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is not null)
        {
            await _cacheService.SetAsync(key, user.Culture, CacheConsts.OneDay, cancellationToken);
            await _localizer.ResetCultureForUserWithChatIdAsync(user.ChatId, cancellationToken);
        }
    }
}
