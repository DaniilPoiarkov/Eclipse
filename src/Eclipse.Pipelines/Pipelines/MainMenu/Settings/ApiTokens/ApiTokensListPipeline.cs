using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Application.Contracts.Users;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.Stores;
using Eclipse.Localization.Localizers;

using System.Text;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings.ApiTokens;

[Route("Menu:ApiTokens:List", "/api_tokens_list")]
internal sealed class ApiTokensListPipeline : ApiTokensPipelineBase
{
    private readonly IApiTokenService _apiTokenService;

    private readonly IUserService _userService;

    private readonly IMessageStore _messageStore;

    private const int MaxTokensToShow = 5;

    public ApiTokensListPipeline(IApiTokenService apiTokenService, IUserService userService, IMessageStore messageStore)
    {
        _apiTokenService = apiTokenService;
        _userService = userService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(SendList);
        RegisterStage(HandleUpdate);
    }

    private async Task<IResult> SendList(MessageContext context, CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Error"]);
        }

        var tokensResult = await _apiTokenService.GetListAsync(userResult.Value.Id, cancellationToken);

        if (!tokensResult.IsSuccess)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Error"]);
        }

        var tokens = tokensResult.Value.Take(MaxTokensToShow).ToList();

        if (tokens.Count == 0)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Pipelines:ApiTokens:List:Empty"]);
        }

        return Menu(new InlineKeyboardMarkup(BuildButtons(tokens)), BuildMessage(tokens));
    }

    private async Task<IResult> HandleUpdate(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetLatestBotMessage(context.ChatId, typeof(ApiTokensListPipeline), cancellationToken);

        if (context.Value.Equals("go_back"))
        {
            return RemoveMenuAndRedirect<MainMenuPipeline>(message);
        }

        var tokenId = context.Value.ToGuid();

        if (tokenId == Guid.Empty)
        {
            return InvalidActionOrRedirect(context, message);
        }

        var userResult = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            return MenuAndClearPrevious(ApiTokensMenuButtons, message, Localizer["Error"]);
        }

        var revokeResult = await _apiTokenService.RevokeAsync(userResult.Value.Id, tokenId, cancellationToken);

        if (!revokeResult.IsSuccess)
        {
            return MenuAndClearPrevious(ApiTokensMenuButtons, message, Localizer["Error"]);
        }

        var tokensResult = await _apiTokenService.GetListAsync(userResult.Value.Id, cancellationToken);

        var tokens = tokensResult.IsSuccess
            ? tokensResult.Value.Take(MaxTokensToShow).ToList()
            : [];

        if (tokens.Count == 0)
        {
            return MenuAndClearPrevious(ApiTokensMenuButtons, message, Localizer["Pipelines:ApiTokens:List:Revoked"]);
        }

        RegisterStage(HandleUpdate);

        if (message is null)
        {
            return Menu(new InlineKeyboardMarkup(BuildButtons(tokens)), BuildMessage(tokens));
        }

        return Edit(message.MessageId, BuildMessage(tokens), new InlineKeyboardMarkup(BuildButtons(tokens)));
    }

    private IResult InvalidActionOrRedirect(MessageContext context, Message? message)
    {
        try
        {
            var localized = Localizer.ToLocalizableString(context.Value);

            return localized switch
            {
                "Menu:ApiTokens:List" => RemoveMenuAndRedirect<ApiTokensListPipeline>(message),
                "Menu:ApiTokens:Create" => RemoveMenuAndRedirect<CreateApiTokenPipeline>(message),
                "Menu:MainMenu:Settings" => RemoveMenuAndRedirect<SettingsPipeline>(message),
                _ => MenuAndClearPrevious(ApiTokensMenuButtons, message, Localizer["Error"]),
            };
        }
        catch
        {
            return MenuAndClearPrevious(ApiTokensMenuButtons, message, Localizer["Error"]);
        }
    }

    private string BuildMessage(List<ApiTokenDto> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            sb.AppendLine($"🔑 {token.Name}")
                .AppendLine($"{Localizer["ExpiresAt"]}: {token.ExpiresAt:yyyy-MM-dd}")
                .AppendLine();
        }

        sb.Append(Localizer["Pipelines:ApiTokens:List:ClickToRevoke"]);

        return sb.ToString();
    }

    private InlineKeyboardButton[][] BuildButtons(List<ApiTokenDto> tokens)
    {
        return
        [
            .. tokens.Select(t => new[] {InlineKeyboardButton.WithCallbackData(
                t.Name.Length < 30
                    ? t.Name
                    : $"{t.Name[..28]}..",
                t.Id.ToString())}
            ),
            [InlineKeyboardButton.WithCallbackData(Localizer["GoBack"], "go_back")],
        ];
    }
}
