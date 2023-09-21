using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Builder;
using Eclipse.Localization.Localizers;

using Telegram.Bot;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Suggest", "/suggest")]
public class SuggestPipeline : EclipsePipelineBase
{
    private readonly ITelegramBotClient _botClient;

    private readonly InfrastructureOptions _options;

    private readonly ISuggestionsSheetsService _sheetsService;

    private readonly ILocalizer _localizer;

    public SuggestPipeline(ITelegramBotClient botClient, InfrastructureOptions options, ISuggestionsSheetsService sheetsService, ILocalizer localizer)
    {
        _botClient = botClient;
        _options = options;
        _sheetsService = sheetsService;
        _localizer = localizer;
    }

    protected override void Initialize()
    {
        RegisterStage(SendInfo);
        RegisterStage(RecieveIdea);
    }

    protected IResult SendInfo(MessageContext context)
    {
        var greetings = _localizer["Pipelines:Suggest:Greetings"].Split(';', StringSplitOptions.RemoveEmptyEntries);
        var greeting = greetings[Random.Shared.Next(0, greetings.Length)];

        return Text(string.Format(_localizer["Pipelines:Suggest"], greeting));
    }

    protected async Task<IResult> RecieveIdea(MessageContext context, CancellationToken cancellationToken = default)
    {
        if (context.Value.Equals("/cancel", StringComparison.CurrentCultureIgnoreCase))
        {
            return Menu(MainMenuButtons, _localizer["Pipelines:Suggest:AsYouWish"]);
        }

        if (string.IsNullOrEmpty(context.Value))
        {
            return Menu(MainMenuButtons, _localizer["Pipelines:Suggest:Error"]);
        }

        var options = _options.TelegramOptions;

        var message = $"Suggestion from {context.User.Name}{context.User.Username.FormattedOrEmpty(s => $", @{s}")}:" +
            $"\n{context.Value}";

        var suggestionDto = new SuggestionDto
        {
            Id = Guid.NewGuid(),
            Text = context.Value,
            TelegramUserId = context.User.Id,
            CreatedAt = DateTime.UtcNow,
        };

        _sheetsService.Add(suggestionDto);

        await _botClient.SendTextMessageAsync(options.Chat, message, cancellationToken: cancellationToken);

        return Menu(MainMenuButtons, _localizer["Pipelines:Suggest:Success"]);
    }
}
