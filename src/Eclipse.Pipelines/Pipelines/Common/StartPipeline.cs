using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Stores.Messages;

using Microsoft.Extensions.Options;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Start", "/start")]
public sealed class StartPipeline : EclipsePipelineBase
{
    private readonly ICultureTracker _cultureTracker;

    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private readonly IOptions<CultureList> _cultures;

    private readonly ICurrentCulture _currentCulture;

    private const string _prefix = "Pipelines:Start";

    private const int _languagesChunk = 2;

    public StartPipeline(
        ICultureTracker cultureTracker,
        IUserService userService,
        IMessageStore messageStore,
        IOptions<CultureList> cultures,
        ICurrentCulture currentCulture)
    {
        _cultureTracker = cultureTracker;
        _userService = userService;
        _messageStore = messageStore;
        _cultures = cultures;
        _currentCulture = currentCulture;
    }

    protected override void Initialize()
    {
        RegisterStage(ConfigureAccountAsync);
        RegisterStage(SetLanguage);
    }

    private async Task<IResult> ConfigureAccountAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (user.IsSuccess)
        {
            FinishPipeline();
            return SendGreeting(context);
        }

        var buttons = _cultures.Value
            .Select(l => InlineKeyboardButton.WithCallbackData(Localizer[l.Culture], l.Code))
            .Chunk(_languagesChunk)
            .ToArray();

        return Menu(buttons, Localizer[$"{_prefix}:SetLanguage"]);
    }

    private async Task<IResult> SetLanguage(MessageContext context, CancellationToken cancellationToken = default)
    {
        var message = await _messageStore.GetOrDefaultAsync(new MessageKey(context.ChatId), cancellationToken);

        if (!SupportedLanguage(context))
        {
            return SendGreeting(context, message);
        }

        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return SendGreeting(context, message);
        }

        var user = result.Value;

        if (user.Culture == context.Value)
        {
            return SendGreeting(context, message);
        }

        var model = new UserPartialUpdateDto
        {
            CultureChanged = true,
            Culture = context.Value,
        };

        await _userService.UpdatePartialAsync(user.Id, model, cancellationToken);

        await _cultureTracker.ResetAsync(context.ChatId, context.Value, cancellationToken);

        using var _ = _currentCulture.UsingCulture(context.Value);

        return SendGreeting(context, message);
    }

    private IResult SendGreeting(MessageContext context, Message? message = null)
    {
        return MenuAndRemoveOptions(Localizer[_prefix, context.User.Name.TrimEnd()], message);
    }

    private bool SupportedLanguage(MessageContext context)
    {
        return _cultures.Value.Exists(l => l.Code.Equals(context.Value));
    }

    private IResult MenuAndRemoveOptions(string text, Message? message)
    {
        if (message is null)
        {
            return Menu(MainMenuButtons, text);
        }

        return Multiple(
            Edit(message.MessageId, InlineKeyboardMarkup.Empty()),
            Menu(MainMenuButtons, text)
        );
    }
}
