using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Infrastructure.Cache;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    private static readonly Lazy<IEclipseLocalizer> _localizer = new(GetService<IEclipseLocalizer>);
    protected static IEclipseLocalizer Localizer => _localizer.Value;

    protected static IReadOnlyCollection<KeyboardButton> MainMenuButtons => new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]),
        new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]),
        new KeyboardButton(Localizer["Menu:MainMenu:Language"]),
    };

    private static TService GetService<TService>()
        where TService : class
    {
        using var scope = CachedServiceProvider.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }

    public override async Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default)
    {
        var cache = GetService<ICacheService>();

        var key = new CacheKey($"lang-{context.ChatId}");

        var culture = cache.Get<string>(key);

        if (culture is not null)
        {
            return await base.RunNext(context, cancellationToken);
        }

        var identityService = GetService<IIdentityUserService>();
        
        try
        {
            var user = await identityService.GetByChatIdAsync(context.ChatId, cancellationToken);
            cache.Set(key, user.Culture);
            Localizer.CheckCulture(user.ChatId);
        }
        catch (Exception) { }

        return await base.RunNext(context, cancellationToken);
    }
}
