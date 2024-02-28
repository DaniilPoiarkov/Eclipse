using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Common.Telegram;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using Microsoft.Extensions.Options;

using Telegram.Bot;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Suggest", "/suggest")]
public sealed class SuggestPipeline : EclipsePipelineBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly IOptions<TelegramOptions> _options;

    private readonly ISuggestionsService _suggestionsService;

    public SuggestPipeline(ITelegramBotClient botClient, IOptions<TelegramOptions> options, ISuggestionsService suggestionsService)
    {
        _botClient = botClient;
        _options = options;
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

        var message = $"Suggestion from {context.User.Name}{context.User.Username.FormattedOrEmpty(s => $", @{s}")}:{Environment.NewLine}{context.Value}";

        var request = new CreateSuggestionRequest
        {
            Text = context.Value,
            TelegramUserId = context.User.Id,
        };

        await _suggestionsService.CreateAsync(request, cancellationToken);

        // TODO: Rework, move to the service
        await _botClient.SendTextMessageAsync(_options.Value.Chat, message, cancellationToken: cancellationToken);

        return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:Success"]);
    }
}
