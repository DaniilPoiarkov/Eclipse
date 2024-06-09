using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Common.Cache;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Options.Languages;
using Eclipse.Pipelines.Pipelines.MainMenu.Settings;
using Eclipse.Pipelines.Stores.Messages;

using Microsoft.Extensions.Options;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Language;

[Route("Menu:Settings:Language", "/settings_language")]
internal sealed class ChangeLanguagePipeline : SettingsPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private readonly IOptions<LanguageList> _languages;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:Language";

    private static readonly int _languagesChunk = 2;

    public ChangeLanguagePipeline(ICacheService cacheService, IUserService userService, IMessageStore messageStore, IOptions<LanguageList> languages)
    {
        _cacheService = cacheService;
        _userService = userService;
        _messageStore = messageStore;
        _languages = languages;
    }

    protected override void Initialize()
    {
        RegisterStage(SendAvailableLanguages);
        RegisterStage(SetLanguage);
    }

    private IResult SendAvailableLanguages(MessageContext context)
    {
        var buttons = _languages.Value
            .Select(l => InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:{l.Language}"], l.Code))
            .Chunk(_languagesChunk);

        return Menu(buttons, Localizer[$"{_pipelinePrefix}:Choose"]);
    }

    private async Task<IResult> SetLanguage(MessageContext context, CancellationToken cancellationToken = default)
    {
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        if (!SupportedLanguage(context))
        {
            return MenuAndRemoveOptions(
                Localizer[$"{_pipelinePrefix}:Unsupported"],
                message?.MessageId);
        }

        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return MenuAndRemoveOptions(
                Localizer.LocalizeError(result.Error),
                message?.MessageId);
        }

        var user = result.Value;

        if (user.Culture == context.Value)
        {
            return MenuAndRemoveOptions(
                Localizer[$"{_pipelinePrefix}:Changed"],
                message?.MessageId);
        }

        var updateDto = new UserUpdateDto
        {
            Culture = context.Value,
        };

        var updateResult = await _userService.UpdateAsync(user.Id, updateDto, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return MenuAndRemoveOptions(
                Localizer.LocalizeError(updateResult.Error),
                message?.MessageId);
        }

        var key = new CacheKey($"lang-{context.ChatId}");

        await _cacheService.DeleteAsync(key, cancellationToken);
        await _cacheService.SetAsync(key, context.Value, TimeSpan.FromDays(1), cancellationToken);

        await Localizer.ResetCultureForUserWithChatIdAsync(context.ChatId, cancellationToken);

        return MenuAndRemoveOptions(
            Localizer[$"{_pipelinePrefix}:Changed"],
            message?.MessageId);
    }

    private bool SupportedLanguage(MessageContext context)
    {
        return _languages.Value
            .Exists(l => l.Code.Equals(context.Value));
    }
}
