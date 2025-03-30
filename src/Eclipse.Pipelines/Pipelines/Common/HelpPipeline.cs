using Eclipse.Application.Contracts.Telegram;
using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;

using System.Text;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("", "/help")]
public sealed class HelpPipeline : EclipsePipelineBase
{
    private readonly ICommandService _commandService;

    public HelpPipeline(ICommandService commandService)
    {
        _commandService = commandService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetInfo);
    }

    private async Task<IResult> GetInfo(MessageContext context, CancellationToken cancellationToken = default)
    {
        var commands = await _commandService.GetList(cancellationToken);

        var sb = new StringBuilder($"{Localizer["Pipelines:Common:Help"]}:")
            .AppendLine()
            .AppendLine();

        foreach (var command in commands)
        {
            sb.AppendLine($"/{command.Command} - {command.Description}");
        }

        return Text(sb.ToString());
    }
}
