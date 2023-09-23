using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Language;

[Route("Menu:MainMenu:Language", "/language")]
internal class ChangeLanguagePipeline : EclipsePipelineBase
{
    private readonly ICacheService _cacheService;

    public ChangeLanguagePipeline(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendAvailableLanguages);
        RegisterStage(SetLanguage);
    }

    private IResult SetLanguage(MessageContext context)
    {
        if (!SupportedLanguage(context))
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Language:Unsupported"]);
        }

        var key = new CacheKey($"lang-{context.ChatId}");

        _cacheService.Delete(key);
        _cacheService.Set(key, context.Value);

        Localizer.SetCulture(context.ChatId);

        return Menu(MainMenuButtons, Localizer["Pipelines:Language:Changed"]);

        static bool SupportedLanguage(MessageContext context)
        {
            return context.Value.Equals("en") || context.Value.Equals("uk");
        }
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

    
}
