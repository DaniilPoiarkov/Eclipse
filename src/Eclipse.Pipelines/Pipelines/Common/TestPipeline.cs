using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Domain.Users;
using Eclipse.Pipelines.Attributes;

using System.Reflection;

namespace Eclipse.Pipelines.Pipelines.Common;

[AdminOnly]
[Route("", "/test")]
internal sealed class TestPipeline : EclipsePipelineBase
{
    private static readonly DateTime _release = new FileInfo(typeof(TestPipeline).GetTypeInfo().Assembly.Location).LastWriteTimeUtc;

    private readonly UserManager _userManager;

    public TestPipeline(UserManager userManager)
    {
        _userManager = userManager;
    }

    protected override void Initialize()
    {
        RegisterStage(TriggerTestEventAsync);
    }

    private async Task<IResult> TriggerTestEventAsync(MessageContext context, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByChatIdAsync(context.ChatId, cancellationToken);

        if (user is null)
        {
            return Text($"{_release:dd.MM.yyyy HH:mm} | Test not triggered.");
        }

        user.TriggerTestEvent();

        await _userManager.UpdateAsync(user, cancellationToken);

        return Text($"{_release:dd.MM.yyyy HH:mm} | Test triggered successfully.");
    }
}
