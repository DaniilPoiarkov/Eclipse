using Eclipse.Application.Contracts.Localizations;
using Eclipse.Common.Cache;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

namespace Eclipse.Application.Localizations;

internal sealed class EclipseLocalizer : IEclipseLocalizer
{
    private readonly ILocalizer _localizer;

    private readonly ICacheService _cacheService;

    private string Culture { get; set; } = "uk";

    public EclipseLocalizer(ILocalizer localizer, ICacheService cacheService)
    {
        _localizer = localizer;
        _cacheService = cacheService;
    }

    public string this[string key] => _localizer[key, Culture];

    public async Task ResetCultureForUserWithChatIdAsync(long chatId, CancellationToken cancellationToken = default)
    {
        // TODO: Remove hardcoded culture
        Culture = await _cacheService.GetAsync<string>(
            new CacheKey($"lang-{chatId}"), cancellationToken
        ) ?? "uk";
    }

    public string ToLocalizableString(string value) =>
        _localizer.ToLocalizableString(value);

    public string FormatLocalizedException(LocalizedException exception, string? culture = null) =>
        _localizer.FormatLocalizedException(exception, culture ?? Culture);
}
