using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using System.Text;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("", "/help")]
public sealed class HelpPipeline : EclipsePipelineBase
{
    private static readonly string _info = "Here what I can do:\r\n\r\n";

    private readonly ICommandService _commandService;

    public HelpPipeline(ICommandService commandService)
    {
        _commandService = commandService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetInfo);
    }

    protected virtual async Task<IResult> GetInfo(MessageContext context, CancellationToken cancellationToken = default)
    {
        var commands = await _commandService.GetList(cancellationToken);

        var sb = new StringBuilder(_info);

        foreach (var command in commands)
        {
            sb.AppendLine($"/{command.Command} - {command.Description}");
        }

        return Text(sb.ToString());
    }
}
