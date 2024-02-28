using Eclipse.Core.Attributes;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Test", "/test")]
internal sealed class TestPipeline : EclipsePipelineBase
{
    private readonly IdentityUserManager _identityUserManager;

    public TestPipeline(IdentityUserManager identityUserManager)
    {
        _identityUserManager = identityUserManager;
    }

    protected override void Initialize()
    {
        RegisterStage(async (context, token) =>
        {
            var user = await _identityUserManager.FindByChatIdAsync(context.ChatId, token);

            if (user is null)
            {
                return Text("Not ok:(");
            }

            user.Test();

            await _identityUserManager.UpdateAsync(user, token);
            return Text("OK:)");
        });
    }
}
