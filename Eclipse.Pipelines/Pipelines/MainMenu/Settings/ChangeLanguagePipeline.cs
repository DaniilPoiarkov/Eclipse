using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Cache;
using Eclipse.Common.Results;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
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

    private static readonly string _pipelinePrefix = "Pipelines:Settings:Language";

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
            InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:English"], "en"),
            InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:Ukrainian"], "uk"),
        };

        return Menu(buttons, Localizer[$"{_pipelinePrefix}:Choose"]);
    }

    private async Task<IResult> SetLanguage(MessageContext context, CancellationToken cancellationToken = default)
    {
        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        if (!SupportedLanguage(context))
        {
            return MenuAndRemoveOptions(
                Localizer[$"{_pipelinePrefix}:Unsupported"],
                message?.MessageId);
        }

        var result = await _identityUserService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            // TODO: Remove
            throw new EntityNotFoundException(typeof(IdentityUser));
        }

        var user = result.Value;

        if (user.Culture == context.Value)
        {
            return MenuAndRemoveOptions(
                Localizer[$"{_pipelinePrefix}:Changed"],
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

        return MenuAndRemoveOptions(
            Localizer[$"{_pipelinePrefix}:Changed"],
            message?.MessageId);

        static bool SupportedLanguage(MessageContext context)
        {
            return context.Value.Equals("en") || context.Value.Equals("uk");
        }
    }
}
