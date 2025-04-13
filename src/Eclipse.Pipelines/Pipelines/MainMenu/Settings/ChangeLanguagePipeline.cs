using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Stores.Messages;

using Microsoft.Extensions.Options;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:Settings:Language", "/settings_language")]
internal sealed class ChangeLanguagePipeline : SettingsPipelineBase
{
    private readonly ICultureTracker _cultureTracker;

    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private readonly IOptions<CultureList> _cultures;

    private readonly ICurrentCulture _currentCulture;

    private static readonly string _pipelinePrefix = "Pipelines:Settings:Language";

    private static readonly int _languagesChunk = 2;

    public ChangeLanguagePipeline(ICultureTracker cultureTracker, IUserService userService, IMessageStore messageStore, IOptions<CultureList> cultures, ICurrentCulture currentCulture)
    {
        _cultureTracker = cultureTracker;
        _userService = userService;
        _messageStore = messageStore;
        _cultures = cultures;
        _currentCulture = currentCulture;
    }

    protected override void Initialize()
    {
        RegisterStage(SendAvailableLanguages);
        RegisterStage(SetLanguage);
    }

    private IResult SendAvailableLanguages(MessageContext context)
    {
        var buttons = _cultures.Value
            .Select(l => InlineKeyboardButton.WithCallbackData(Localizer[$"{_pipelinePrefix}:{l.Culture}"], l.Code))
            .Chunk(_languagesChunk)
            .ToArray();

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

        var updateDto = new UserPartialUpdateDto
        {
            CultureChanged = true,
            Culture = context.Value,
        };

        var updateResult = await _userService.UpdatePartialAsync(user.Id, updateDto, cancellationToken);

        if (!updateResult.IsSuccess)
        {
            return MenuAndRemoveOptions(
                Localizer.LocalizeError(updateResult.Error),
                message?.MessageId);
        }

        await _cultureTracker.ResetAsync(context.ChatId, context.Value, cancellationToken);

        using var _ = _currentCulture.UsingCulture(context.Value);

        return MenuAndRemoveOptions(
            Localizer[$"{_pipelinePrefix}:Changed"],
            message?.MessageId);
    }

    private bool SupportedLanguage(MessageContext context)
    {
        return _cultures.Value
            .Exists(l => l.Code.Equals(context.Value));
    }
}
