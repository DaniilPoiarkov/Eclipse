using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Culture;

using Microsoft.Extensions.Options;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Start", "/start")]
public sealed class StartPipeline : EclipsePipelineBase
{
    private readonly ICultureTracker _cultureTracker;

    private readonly IUserService _userService;

    private readonly IOptions<CultureList> _cultures;

    private readonly ICurrentCulture _currentCulture;

    private const string _prefix = "Pipelines:Start";

    private const int _languagesChunk = 2;

    public StartPipeline(
        ICultureTracker cultureTracker,
        IUserService userService,
        IOptions<CultureList> cultures,
        ICurrentCulture currentCulture)
    {
        _cultureTracker = cultureTracker;
        _userService = userService;
        _cultures = cultures;
        _currentCulture = currentCulture;
    }

    protected override void Initialize()
    {
        RegisterStage(ConfigureLanguage);
        RegisterStage(ConfigureGmt);
        RegisterStage(FinishOnboarding);
    }

    private async Task<IResult> ConfigureLanguage(MessageContext context, CancellationToken cancellationToken)
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

    private async Task<IResult> ConfigureGmt(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (!SupportedLanguage(context))
        {
            return RequestTime();
        }

        var result = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!result.IsSuccess)
        {
            return RequestTime();
        }

        var user = result.Value;

        if (user.Culture == context.Value)
        {
            return RequestTime();
        }

        var model = new UserPartialUpdateDto
        {
            CultureChanged = true,
            Culture = context.Value,
        };

        await _userService.UpdatePartialAsync(user.Id, model, cancellationToken);

        await _cultureTracker.ResetAsync(context.ChatId, context.Value, cancellationToken);

        using var _ = _currentCulture.UsingCulture(context.Value);

        return RequestTime();
    }

    private async Task<IResult> FinishOnboarding(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return SendGreeting(context);
        }

        if (!context.Value.TryParseAsTimeOnly(out var time))
        {
            RegisterStage(FinishOnboarding);
            return Text(Localizer[$"{_prefix}:CannotParseTime"]);
        }

        var user = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!user.IsSuccess)
        {
            return SendGreeting(context);
        }

        var update = new UserPartialUpdateDto
        {
            Gmt = time,
            GmtChanged = true,
        };

        await _userService.UpdatePartialAsync(user.Value.Id, update, cancellationToken);

        return SendGreeting(context);
    }

    private IResult RequestTime()
    {
        return Menu(new ReplyKeyboardRemove(), Localizer[$"{_prefix}:SetTime"]);
    }

    private IResult SendGreeting(MessageContext context)
    {
        return Menu(MainMenuButtons, Localizer[_prefix, context.User.Name.TrimEnd()]);
    }

    private bool SupportedLanguage(MessageContext context)
    {
        return _cultures.Value.Exists(l => l.Code.Equals(context.Value));
    }
}
