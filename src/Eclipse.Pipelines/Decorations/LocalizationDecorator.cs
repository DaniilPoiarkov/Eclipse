using Eclipse.Common.Caching;
using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;

using System.Globalization;

namespace Eclipse.Pipelines.Decorations;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly IUserRepository _userRepository;

    private readonly ICurrentCulture _currentCulture;

    private readonly ICacheService _cacheService;

    public LocalizationDecorator(IUserRepository userRepository, ICurrentCulture currentCulture, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _currentCulture = currentCulture;
        _cacheService = cacheService;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        string? culture = await _cacheService.GetOrCreateAsync(
            $"lang-{context.ChatId}",
            () => GetCultureAsync(context, cancellationToken),
            options,
            cancellationToken
        );

        CultureInfo? cultureInfo = null;

        if (!culture.IsNullOrEmpty())
        {
            cultureInfo = CultureInfo.GetCultureInfo(culture);
        }

        using var _ = _currentCulture.UsingCulture(cultureInfo);

        return await execution(context, cancellationToken);
    }

    private async Task<string?> GetCultureAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is not null)
        {
            return user.Culture;
        }

        return null;
    }
}
