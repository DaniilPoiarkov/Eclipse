using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Extensions;
using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

using System.Text;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.View;

[Route("Menu:AdminMenu:View:Suggestions", "/admin_view_suggestions")]
internal class ViewSuggestionsPipeline : AdminPipelineBase
{
    private readonly ISuggestionsService _suggestionsService;

    public ViewSuggestionsPipeline(ISuggestionsService suggestionsService)
    {
        _suggestionsService = suggestionsService;
    }

    protected override void Initialize()
    {
        RegisterStage(GetInfo);
    }

    private async Task<IResult> GetInfo(MessageContext context, CancellationToken cancellationToken = default)
    {
        var suggestions = await _suggestionsService.GetWithUserInfo(cancellationToken);

        var tabs = new string('=', 5);

        var sb = new StringBuilder($"{tabs} {Localizer["Suggestions"].ToUpper()} {tabs}")
            .AppendLine()
            .AppendLine();

        foreach (var suggestion in suggestions)
        {
            sb.AppendLine(suggestion.Text);

            if (suggestion.User is not null)
            {
                sb.AppendLine($"{suggestion.User.ChatId} | {suggestion.User.Name} {suggestion.User.Username.FormattedOrEmpty(s => $"| @{s}")}");
            }

            sb.AppendLine($"{Localizer["CreatedAt"]}: {suggestion.CreatedAt.ToString("dd.MM - HH:mm")}");
            sb.AppendLine();
        }

        return Text(sb.ToString());
    }
}
