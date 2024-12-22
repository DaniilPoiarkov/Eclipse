using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;

using System.Globalization;

namespace Eclipse.Pipelines.Decorations;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly IUserRepository _userRepository;

    private readonly ICultureTracker _cultureTracker;

    private readonly ICurrentCulture _currentCulture;

    public LocalizationDecorator(IUserRepository userRepository, ICultureTracker cultureTracker, ICurrentCulture currentCulture)
    {
        _userRepository = userRepository;
        _cultureTracker = cultureTracker;
        _currentCulture = currentCulture;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var culture = await _cultureTracker.GetAsync(context.ChatId, cancellationToken);

        if (culture is null)
        {
            var user = await _userRepository.FindByChatIdAsync(context.ChatId, cancellationToken);

            if (user is not null)
            {
                await _cultureTracker.ResetAsync(context.ChatId, user.Culture, cancellationToken);
                culture = user.Culture;
            }
        }

        CultureInfo? cultureInfo = null;

        if (!culture.IsNullOrEmpty())
        {
            cultureInfo = CultureInfo.GetCultureInfo(culture);
        }

        using var _ = _currentCulture.UsingCulture(cultureInfo);

        return await execution(context, cancellationToken);
    }
}
