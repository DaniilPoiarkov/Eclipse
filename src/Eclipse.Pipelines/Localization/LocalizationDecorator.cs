using Eclipse.Core.Builder;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;

using System.Globalization;

namespace Eclipse.Pipelines.Localization;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly ICurrentCulture _currentCulture;

    private readonly ICultureTracker _cultureTracker;

    public LocalizationDecorator(ICurrentCulture currentCulture, ICultureTracker cultureTracker)
    {
        _currentCulture = currentCulture;
        _cultureTracker = cultureTracker;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        string? culture = await _cultureTracker.GetAsync(context.ChatId, cancellationToken);

        CultureInfo? cultureInfo = null;

        if (!culture.IsNullOrEmpty())
        {
            cultureInfo = CultureInfo.GetCultureInfo(culture);
        }

        using var _ = _currentCulture.UsingCulture(cultureInfo);

        return await execution(context, cancellationToken);
    }
}
