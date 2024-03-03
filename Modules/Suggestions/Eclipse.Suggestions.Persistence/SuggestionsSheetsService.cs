using Eclipse.Application.Sheets;
using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Suggestions.Domain;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Suggestions.Persistence;

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
