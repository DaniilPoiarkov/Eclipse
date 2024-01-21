using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Cache;
using Eclipse.Pipelines.Pipelines.MainMenu.Settings;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Language;

[Route("Menu:Settings:Language", "/settings_language")]
internal class ChangeLanguagePipeline : SettingsPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IIdentityUserService _identityUserService;

    private readonly IMessageStore _messageStore;

    public ChangeLanguagePipeline(ICacheService cacheService, IIdentityUserService identityUserService, IMessageStore messageStore)
    {
        _cacheService = cacheService;
        _identityUserService = identityUserService;
        _messageStore = messageStore;
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
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Settings:Language:English"], "en"),
            InlineKeyboardButton.WithCallbackData(Localizer["Pipelines:Settings:Language:Ukrainian"], "uk"),
        };

        return Menu(buttons, Localizer["Pipelines:Settings:Language:Choose"]);
    }

    private async Task<IResult> SetLanguage(MessageContext context, CancellationToken cancellationToken = default)
    {
        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        if (!SupportedLanguage(context))
        {
            return MenuAndEditedOptionsMessage(
                Localizer["Pipelines:Settings:Language:Unsupported"],
                message?.MessageId);
        }

        var user = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (user.Culture == context.Value)
        {
            return MenuAndEditedOptionsMessage(
                Localizer["Pipelines:Settings:Language:Changed"],
                message?.MessageId);
        }

        var updateDto = new IdentityUserUpdateDto
        {
            Culture = context.Value,
        };

        await _identityUserService.UpdateAsync(user.Id, updateDto, cancellationToken);

        var key = new CacheKey($"lang-{context.ChatId}");

        _cacheService.Delete(key);
        _cacheService.Set(key, context.Value);

        Localizer.CheckCulture(context.ChatId);

        return MenuAndEditedOptionsMessage(
            Localizer["Pipelines:Settings:Language:Changed"],
            message?.MessageId);

        static bool SupportedLanguage(MessageContext context)
        {
            return context.Value.Equals("en") || context.Value.Equals("uk");
        }
    }
}
