using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Shared.IdentityUsers;
using Eclipse.Infrastructure.Cache;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Language;

[Route("Menu:MainMenu:Language", "/language")]
internal class ChangeLanguagePipeline : EclipsePipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IIdentityUserService _identityUserService;

    public ChangeLanguagePipeline(ICacheService cacheService, IIdentityUserService identityUserService)
    {
        _cacheService = cacheService;
        _identityUserService = identityUserService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendAvailableLanguages);
        RegisterStage(SetLanguage);
    }

    private IResult SendAvailableLanguages(MessageContext context)
    {
        var buttons = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Language:English"], "en"),
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Language:Ukrainian"], "uk"),
        };

        return Menu(buttons, Localizer["Pipelines:Language:Choose"]);
    }

    private async Task<IResult> SetLanguage(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (!SupportedLanguage(context))
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Language:Unsupported"]);
        }

        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);
        var updateDto = new IdentityUserUpdateDto
        {
            Culture = context.Value,
        };

        await _identityUserService.UpdateAsync(user.Id, updateDto, cancellationToken);

        var key = new CacheKey($"lang-{context.ChatId}");

        _cacheService.Delete(key);
        _cacheService.Set(key, context.Value);

        Localizer.CheckCulture(context.ChatId);

        return Menu(MainMenuButtons, Localizer["Pipelines:Language:Changed"]);

        static bool SupportedLanguage(MessageContext context)
        {
            return context.Value.Equals("en") || context.Value.Equals("uk");
        }
    }
}
