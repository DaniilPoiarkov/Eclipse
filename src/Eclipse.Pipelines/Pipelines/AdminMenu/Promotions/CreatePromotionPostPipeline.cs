using Eclipse.Application.Contracts.Promotions;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Domain.Promotions;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.Caching;
using Eclipse.Pipelines.Stores;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:Admin:Promotions:Create", "/admin_promotions_create")]
internal sealed class CreatePromotionPostPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    private readonly IPromotionService _promotionService;

    private readonly IMessageStore _messageStore;

    public CreatePromotionPostPipeline(
        ICacheService cacheService,
        ITelegramBotClient botClient,
        IPromotionService promotionService,
        IMessageStore messageStore)
    {
        _cacheService = cacheService;
        _botClient = botClient;
        _promotionService = promotionService;
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(RequestPostMessage);
        RegisterStage(ReviewPostMessageAsync);
        RegisterStage(SetPromotionTitle);
        RegisterStage(AskToAddInlineButton);
        RegisterStage(AskForInlineButtonText);
        RegisterStage(AskForInlineButtonLink);
        RegisterStage(ValidateRedirectUrlAndSave);
    }

    private IResult RequestPostMessage(MessageContext context)
    {
        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:Request"]);
    }

    private async Task<IResult> ReviewPostMessageAsync(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        if (Update.Message is not { })
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes
        };

        await _cacheService.SetAsync(
            $"promotions-create-message-{context.ChatId}",
            Update.Message.Id,
            options,
            cancellationToken
        );

        await _botClient.CopyMessage(context.ChatId, context.ChatId, Update.Message.Id, cancellationToken: cancellationToken);

        return Menu(
            [
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Create:Review:Continue"]) ],
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Create:Review:Cancel"]) ]
            ],
            Localizer["Pipelines:Admin:Promotions:Create:Review"]
        );
    }

    private async Task<IResult> SetPromotionTitle(MessageContext context, CancellationToken cancellationToken)
    {
        var message = await _messageStore.GetLatestBotMessage(context.ChatId, cancellationToken);

        if (!ContinuePromotionProcessing(context))
        {
            FinishPipeline();
            return MenuAndClearPrevious(PromotionsButtons, message, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        return MenuAndClearPrevious(new ReplyKeyboardRemove(), message, Localizer["Pipelines:Admin:Promotions:Create:Title"]);
    }

    private async Task<IResult> AskToAddInlineButton(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        await _cacheService.SetForThreeDaysAsync(
            $"promotions-create-title-{context.ChatId}",
            context.Value,
            context.ChatId,
            cancellationToken: cancellationToken
        );

        return Menu(
            [
                [ new KeyboardButton(Localizer["Yes"]) ],
                [ new KeyboardButton(Localizer["No"]) ]
            ],
            Localizer["Pipelines:Admin:Promotions:Create:InlineButton:Request"]
        );
    }

    private async Task<IResult> AskForInlineButtonText(MessageContext context, CancellationToken cancellationToken)
    {
        try
        {
            var localized = Localizer.ToLocalizableString(context.Value);

            if (localized.Equals("Yes"))
            {
                return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:InlineButton:Text:Request"]);
            }

            if (localized.Equals("No"))
            {
                FinishPipeline();
                return await SavePromotionPostAsync(context, string.Empty, string.Empty, cancellationToken);
            }

            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }
        catch
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }
    }

    private async Task<IResult> AskForInlineButtonLink(MessageContext context, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        if (context.Value.IsNullOrEmpty())
        {
            FinishPipeline();
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        await _cacheService.SetForThreeDaysAsync(
            $"promotions-create-inline-button-text-{context.ChatId}",
            context.Value,
            context.ChatId,
            cancellationToken: cancellationToken
        );

        return Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:InlineButton:Link:Request"]);
    }

    private async Task<IResult> ValidateRedirectUrlAndSave(MessageContext context, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(context.Value, UriKind.Absolute, out _))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Invalid"]);
        }

        var buttonText = await _cacheService.GetOrCreateAsync(
            $"promotions-create-inline-button-text-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        return await SavePromotionPostAsync(context, buttonText, context.Value, cancellationToken);
    }

    private async Task<IResult> SavePromotionPostAsync(MessageContext context, string inlineButtonText, string inlineButtonLink, CancellationToken cancellationToken)
    {
        if (context.Value.Equals("/cancel"))
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:Cancelled"]);
        }

        var messageId = await _cacheService.GetOrCreateAsync(
            $"promotions-create-message-{context.ChatId}",
            () => Task.FromResult<int?>(null),
            cancellationToken: cancellationToken
        );

        if (!messageId.HasValue)
        {
            return Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions:Create:MessageNotFound"]);
        }

        var title = await _cacheService.GetOrCreateAsync(
            $"promotions-create-title-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        var promotion = await _promotionService.Create(
            new CreatePromotionRequest(title, context.ChatId, messageId.GetValueOrDefault(), inlineButtonText, inlineButtonLink),
            cancellationToken
        );

        await _cacheService.SetForThreeDaysAsync(
            $"admin-promotions-promotion-{context.ChatId}",
            promotion.Id,
            context.ChatId,
            cancellationToken: cancellationToken
        );

        return Redirect<PromotionActionsPipeline>(
            Menu(new ReplyKeyboardRemove(), Localizer["Pipelines:Admin:Promotions:Create:Success"])
        );
    }

    private bool ContinuePromotionProcessing(MessageContext context)
    {
        return Localizer.TryConvertToLocalizableString(context.Value, out var localized)
            && localized.Equals("Pipelines:Admin:Promotions:Create:Review:Continue");
    }
}
