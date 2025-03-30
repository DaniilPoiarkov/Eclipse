using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Suggest", "/suggest")]
public sealed class SuggestPipeline : EclipsePipelineBase
{
    private readonly ISuggestionsService _service;

    public SuggestPipeline(ISuggestionsService service)
    {
        _service = service;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(ReceiveIdea);
    }

    private IResult SendInfo(MessageContext context)
    {
        var greeting = Localizer["Pipelines:Suggest:Greetings"].Value
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .GetRandomItem();

        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Suggest", greeting]);
    }

    private async Task<IResult> ReceiveIdea(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.IsNullOrEmpty())
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:Error"]);
        }

        if (context.Value.EqualsCurrentCultureIgnoreCase("/cancel"))
        {
            return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:AsYouWish"]);
        }

        var request = new CreateSuggestionRequest
        {
            Text = context.Value,
            TelegramUserId = context.User.Id,
        };

        await _service.CreateAsync(request, cancellationToken);

        return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:Success"]);
    }
}
