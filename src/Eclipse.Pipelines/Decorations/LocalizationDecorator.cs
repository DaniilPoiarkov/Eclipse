using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;

namespace Eclipse.Pipelines.Decorations;

public sealed class LocalizationDecorator : IPipelineExecutionDecorator
{
    private readonly UserManager _userManager;

    private readonly ICultureTracker _cultureTracker;

    private readonly ICurrentCulture _currentCulture;

    public LocalizationDecorator(UserManager userManager, ICultureTracker cultureTracker, ICurrentCulture currentCulture)
    {
        _userManager = userManager;
        _cultureTracker = cultureTracker;
        _currentCulture = currentCulture;
    }

    public async Task<IResult> Decorate(Func<MessageContext, CancellationToken, Task<IResult>> execution, MessageContext context, CancellationToken cancellationToken = default)
    {
        var culture = await _cultureTracker.GetAsync(context.ChatId, cancellationToken);

        if (culture is null)
        {
            var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

            if (user is not null)
            {
                await _cultureTracker.ResetAsync(context.ChatId, user.Culture, cancellationToken);
                culture = user.Culture;
            }
        }

        using var _ = _currentCulture.UsingCulture(culture ?? _currentCulture.Culture);

        return await execution(context, cancellationToken);
    }
}
