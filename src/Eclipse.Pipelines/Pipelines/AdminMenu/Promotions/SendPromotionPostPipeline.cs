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
        return Text(Localizer["Admin:Promotions:SendPromotionPost:RequestPostMessage"]);
    }

    private async Task<IResult> SendConfirmationCodeAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = Enumerable.Range(0, 6)
            .Select(_ => Random.Shared.Next(0, 10))
            .Aggregate("", (s, i) => $"{s}{i}");

        await _cacheService.SetAsync(
            $"promotions-post-confiramtion-code-{context.ChatId}",
            confirmationCode,
            TimeSpan.FromMinutes(5),
            cancellationToken
        );

        return Text(confirmationCode);
    }

    private async Task<IResult> SendPromotionPostAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var confirmationCode = await _cacheService.GetAsync<string>($"promotions-post-confiramtion-code-{context.ChatId}", cancellationToken);

        if (context.Value.Equals(confirmationCode))
        {
            return Text("Valid");
        }

        return Text("Invalid");
    }
}
