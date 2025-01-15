using Eclipse.Common.Caching;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.UpdateParsing;
using Eclipse.Localization.Localizers;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Post", "/admin_promotions_post")]
internal sealed class SendPromotionPostPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    private readonly ITelegramBotClient _botClient;

    private readonly IUpdateProvider _updateProvider;

    public SendPromotionPostPipeline(ICacheService cacheService, ITelegramBotClient botClient, IUpdateProvider updateProvider)
    {
        _cacheService = cacheService;
        _botClient = botClient;
        _updateProvider = updateProvider;
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
        var update = _updateProvider.Get();

        if (update.Message is not { })
        {
            FinishPipeline();
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:Invalid"]);

        }

        await _botClient.CopyMessage(context.ChatId, context.ChatId, update.Message.Id, cancellationToken: cancellationToken);

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

        await _cacheService.SetAsync(
            $"promotions-post-confiramtion-code-{context.ChatId}",
            confirmationCode,
            CacheConsts.FiveMinutes,
            cancellationToken
        );

        return Text(Localizer["Pipelines:Admin:Promotions:Post:Confirm{0}", confirmationCode]);
    }

    private async Task<IResult> SendPromotionPostAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = await _cacheService.GetAsync<string>($"promotions-post-confiramtion-code-{context.ChatId}", cancellationToken);

        if (!context.Value.Equals(confirmationCode))
        {
            return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:ConfirmationFailed"]);
        }

        // TODO: Start processing.

        return Menu(AdminMenuButtons, Localizer["Pipelines:Admin:Promotions:Post:Confirmed"]);
    }

    private bool ContinuePromotionProcessing(MessageContext context)
    {
        return Localizer.TryConvertToLocalizableString(context.Value, out var localized)
            && localized.Equals("Pipelines:Admin:Promotions:Post:Review:Continue");
    }
}
