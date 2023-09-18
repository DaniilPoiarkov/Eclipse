using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu;

[Route("", "/suggestions_all")]
[AdminOnly]
public class CheckSuggestionsPipeline : PipelineBase
{
    private readonly ISuggestionsService _suggestionsService;

    public CheckSuggestionsPipeline(ISuggestionsService suggestionsService)
    {
        _suggestionsService = suggestionsService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetInfo);
    }

    private IResult GetInfo(MessageContext context)
    {
        var suggestions = _suggestionsService.GetWithUserInfo()
            .Select(s => $"{s.Text}{Environment.NewLine}{s.User?.Id ?? 0}: {s.User?.Username ?? "Not found"}, {s.User?.Name ?? "Not found"}");

        return Text(string.Join($"{Environment.NewLine}{Environment.NewLine}", suggestions));
    }
}
