﻿using Eclipse.Common.Background;
using Eclipse.Common.Caching;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Localization.Localizers;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Post", "/admin_promotions_post")]
internal sealed class SendPromotionPostPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    private readonly IBackgroundJobManager _backgroundJobManager;

    public SendPromotionPostPipeline(
        ICacheService cacheService,
        ITelegramBotClient botClient,
        IBackgroundJobManager backgroundJobManager)
    {
        _cacheService = cacheService;
        _botClient = botClient;
        _backgroundJobManager = backgroundJobManager;
    }

    protected override void Initialize()
    {
        RegisterStage(RequestPostMessage);
        RegisterStage(ReviewPostMessageAsync);
        RegisterStage(SendConfirmationCodeAsync);
        RegisterStage(SendPromotionPostAsync);
    }

    private IResult RequestPostMessage(MessageContext context)
    {
        return Text(Localizer["Pipelines:Admin:Promotions:Post:Request"]);
    }

    private async Task<IResult> ReviewPostMessageAsync(MessageContext context, CancellationToken cancellationToken)
    {
        if (Update.Message is not { })
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:Invalid"]);

        }

        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes
        };

        await _cacheService.SetAsync(
            $"promotions-post-message-{context.ChatId}",
            Update.Message.Id,
            options,
            cancellationToken
        );

        await _botClient.CopyMessage(context.ChatId, context.ChatId, Update.Message.Id, cancellationToken: cancellationToken);

        return Menu(
            [
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Post:Review:Continue"]) ],
                [ new KeyboardButton(Localizer["Pipelines:Admin:Promotions:Post:Review:Cancel"]) ]
            ],
            Localizer["Pipelines:Admin:Promotions:Post:Review"]
        );
    }

    private async Task<IResult> SendConfirmationCodeAsync(MessageContext context, CancellationToken cancellationToken)
    {
        if (!ContinuePromotionProcessing(context))
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:Cancelled"]);
        }

        var confirmationCode = Enumerable.Range(0, 6)
            .Select(_ => Random.Shared.Next(0, 10))
            .Aggregate(string.Empty, (s, i) => $"{s}{i}");

        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes
        };

        await _cacheService.SetAsync(
            $"promotions-post-confiramtion-code-{context.ChatId}",
            confirmationCode,
            options,
            cancellationToken
        );

        return Text(Localizer["Pipelines:Admin:Promotions:Post:Confirm{0}", confirmationCode]);
    }

    private async Task<IResult> SendPromotionPostAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = await _cacheService.GetOrCreateAsync(
            $"promotions-post-confiramtion-code-{context.ChatId}",
            () => Task.FromResult(string.Empty),
            cancellationToken: cancellationToken
        );

        if (!context.Value.Equals(confirmationCode))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:ConfirmationFailed"]);
        }

        var messageId = await _cacheService.GetOrCreateAsync(
            $"promotions-post-message-{context.ChatId}",
            () => Task.FromResult<int>(default),
            cancellationToken: cancellationToken
        );

        if (messageId == default)
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:MessageNotFound"]);
        }

        await _backgroundJobManager.EnqueueAsync<SendPromotionBackgroundJob, SendPromotionBackgroundJobArgs>(
            new SendPromotionBackgroundJobArgs
            {
                FromChatId = context.ChatId,
                MessageId = messageId,
            }
        );

        return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:Confirmed"]);
    }

    private bool ContinuePromotionProcessing(MessageContext context)
    {
        return Localizer.TryConvertToLocalizableString(context.Value, out var localized)
            && localized.Equals("Pipelines:Admin:Promotions:Post:Review:Continue");
    }
}
