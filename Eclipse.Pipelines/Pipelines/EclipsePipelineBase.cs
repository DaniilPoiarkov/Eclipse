using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Infrastructure.Cache;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected static IEclipseLocalizer Localizer
    {
        get
        {
            var currentUser = GetService<ICurrentTelegramUser>().GetCurrentUser();
            var localizer = GetService<IEclipseLocalizer>();
            
            if (currentUser is not null)
            {
                localizer.CheckCulture(currentUser.Id);
            }

            return localizer;
        }
    }

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
    
    // TODO: Enhance this
    //public override async Task<IResult> RunNext(MessageContext context, CancellationToken cancellationToken = default)
    //{
    //    var cache = GetService<ICacheService>();

    //    var key = new CacheKey($"lang-{context.ChatId}");

    //    var culture = cache.Get<string>(key);

    //    if (culture is not null)
    //    {
    //        return await base.RunNext(context, cancellationToken);
    //    }

    //    var userManager = GetService<IdentityUserManager>();
        
    //    var user = await userManager.FindByChatId(context.ChatId, cancellationToken);

    //    if (user is not null)
    //    {
    //        cache.Set(key, user.Culture);
    //        Localizer.CheckCulture(user.ChatId);
    //    }

    //    return await base.RunNext(context, cancellationToken);
    //}
}
