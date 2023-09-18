using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Attributes;

using System.Text;

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
        var suggestions = _suggestionsService.GetWithUserInfo();
        
        var tabs = new string('=', 5);

        var sb = new StringBuilder($"{tabs} SUGGESTIONS {tabs}")
            .AppendLine()
            .AppendLine();

        foreach (var suggestion in suggestions)
        {
            sb.AppendLine(suggestion.Text);
            
            if (suggestion.User is not null)
            {
                sb.AppendLine($"{suggestion.User.Id} | @{suggestion.User.Username} | {suggestion.User.Name}");
            }

            sb.AppendLine($"Created at: {suggestion.CreatedAt.ToString("dd.MM - HH:mm")}");
            sb.AppendLine();
        }

        return Text(sb.ToString());
    }
}
