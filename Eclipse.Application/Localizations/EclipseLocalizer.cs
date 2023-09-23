using Eclipse.Application.Contracts.Localizations;
using Eclipse.Infrastructure.Cache;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

namespace Eclipse.Application.Localizations;

internal class EclipseLocalizer : IEclipseLocalizer
{
    private readonly ILocalizer _localizer;

    private readonly ICacheService _cacheService;

    private string Culture { get; set; } = "uk";

    public EclipseLocalizer(ILocalizer localizer, ICacheService cacheService)
    {
        _localizer = localizer;
        _cacheService = cacheService;
    }

    public string this[string key]
    {
        get
        {
            return _localizer[key, Culture];
        }
    }

    public void CheckCulture(long chatId)
    {
        Culture = _cacheService.Get<string>(new CacheKey($"lang-{chatId}")) ?? "uk";
    }

    public string ToLocalizableString(string value) =>
        _localizer.ToLocalizableString(value);

    public string FormatLocalizedException(LocalizedException exception, string? culture = null) =>
        _localizer.FormatLocalizedException(exception, culture ?? Culture);
}
