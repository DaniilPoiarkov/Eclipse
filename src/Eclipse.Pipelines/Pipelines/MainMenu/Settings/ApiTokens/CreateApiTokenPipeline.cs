using Eclipse.Application.Contracts.ApiTokens;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Localizations;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Shared.ApiTokens;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings.ApiTokens;

[Route("Menu:ApiTokens:Create", "/api_tokens_create")]
internal sealed class CreateApiTokenPipeline : ApiTokensPipelineBase
{
    private readonly IApiTokenService _apiTokenService;

    private readonly IUserService _userService;

    public CreateApiTokenPipeline(IApiTokenService apiTokenService, IUserService userService)
    {
        _apiTokenService = apiTokenService;
        _userService = userService;
    }

    protected override void Initialize()
    {
        RegisterStage(PromptName);
        RegisterStage(CreateToken);
    }

    private async Task<IResult> PromptName(MessageContext context, CancellationToken cancellationToken)
    {
        var userResult = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Error"]);
        }

        var tokensResult = await _apiTokenService.GetListAsync(userResult.Value.Id, cancellationToken);

        if (tokensResult.IsSuccess && tokensResult.Value.Count >= ApiTokensConsts.MaxCount)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["ApiToken:LimitReached", ApiTokensConsts.MaxCount]);
        }

        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:ApiTokens:Create:EnterName"]);
    }

    private async Task<IResult> CreateToken(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Okay"]);
        }

        var userResult = await _userService.GetByChatIdAsync(context.ChatId, cancellationToken);

        if (!userResult.IsSuccess)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer["Error"]);
        }

        var result = await _apiTokenService.CreateAsync(userResult.Value.Id, new CreateApiTokenDto(context.Value), cancellationToken);

        if (!result.IsSuccess)
        {
            FinishPipeline();
            return Menu(ApiTokensMenuButtons, Localizer.LocalizeError(result.Error));
        }

        var created = result.Value;

        return Menu(
            ApiTokensMenuButtons,
            Localizer["Pipelines:ApiTokens:Create:Success{Plaintext}{ExpiresAt}", created.Plaintext, created.Token.ExpiresAt.ToString("yyyy-MM-dd")]
        );
    }
}
