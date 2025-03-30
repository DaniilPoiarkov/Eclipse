using Eclipse.Core.Provider;
using Eclipse.Localization.Localizers;

using Microsoft.Extensions.Localization;

namespace Eclipse.Pipelines.Localization;

public sealed class LocalizedKeywordMapper : IKeywordMapper
{
    private readonly IStringLocalizer<LocalizedKeywordMapper> _stringLocalizer;

    public LocalizedKeywordMapper(IStringLocalizer<LocalizedKeywordMapper> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public string Map(string keyword)
    {
        return _stringLocalizer.ToLocalizableString(keyword);
    }
}
