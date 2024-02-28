using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Domain.Suggestions;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets;

internal sealed class SuggestionsSheetsService : EclipseSheetsService<Suggestion>
{
    public SuggestionsSheetsService(
        ISheetsService service,
        IObjectParser<Suggestion> parser,
        IConfiguration configuration,
        IEventBus eventBus)
        : base(service, parser, configuration, eventBus)
    {
    }

    protected override string Range => Configuration["Sheets:SuggestionsRange"]!;
}
