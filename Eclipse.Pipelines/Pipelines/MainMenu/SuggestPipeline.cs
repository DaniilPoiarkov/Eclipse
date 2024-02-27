﻿using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
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

    private readonly ISuggestionsSheetsService _sheetsService;

    public SuggestPipeline(ITelegramBotClient botClient, IOptions<TelegramOptions> options, ISuggestionsSheetsService sheetsService)
    {
        _botClient = botClient;
        _options = options;
        _sheetsService = sheetsService;
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

        var suggestionDto = new SuggestionDto
        {
            Id = Guid.NewGuid(),
            Text = context.Value,
            TelegramUserId = context.User.Id,
            CreatedAt = DateTime.UtcNow,
        };

        _sheetsService.Add(suggestionDto);

        await _botClient.SendTextMessageAsync(_options.Value.Chat, message, cancellationToken: cancellationToken);

        return Menu(MainMenuButtons, Localizer["Pipelines:Suggest:Success"]);
    }
}
