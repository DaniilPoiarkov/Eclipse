using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Suggest", "/suggest")]
public sealed class SuggestPipeline : EclipsePipelineBase
{
    private readonly ISuggestionsService _suggestionsService;

    public SuggestPipeline(ISuggestionsService suggestionsService)
    {
        _suggestionsService = suggestionsService;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(RecieveIdea);
    }

    private IResult SendInfo(MessageContext context)
    {
        var greeting = Localizer!["Pipelines:Suggest:Greetings"]
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .GetRandomItem();

        return Text(string.Format(Localizer["Pipelines:Suggest"], greeting));
    }

    private async Task<IResult> RecieveIdea(MessageContext context, CancellationToken cancellationToken = default)
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

        await _suggestionsService.CreateAsync(request, cancellationToken);

        return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:Success"]);
    }
}
