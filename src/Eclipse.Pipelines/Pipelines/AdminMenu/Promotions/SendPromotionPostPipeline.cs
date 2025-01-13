using Eclipse.Common.Caching;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Post", "/admin_promotions_post")]
internal sealed class SendPromotionPostPipeline : AdminPipelineBase
{
    private readonly ICacheService _cacheService;

    public SendPromotionPostPipeline(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    protected override void Initialize()
    {
        RegisterStage(RequestPostMessage);
        RegisterStage(SendConfirmationCodeAsync);
        RegisterStage(SendPromotionPostAsync);
    }

    private IResult RequestPostMessage(MessageContext context)
    {
        return Text(Localizer["Pipelines:Admin:Promotions:Post:Request"]);
    }

    private async Task<IResult> SendConfirmationCodeAsync(MessageContext context, CancellationToken cancellationToken)
    {
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

        if (context.Value.Equals(confirmationCode))
        {
            return Text(Localizer["Pipelines:Admin:Promotions:Post:Confirmed"]);
        }

        return Text(Localizer["Pipelines:Admin:Promotions:Post:ConfirmationFailed"]);
    }
}
