using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.Suggestions;

internal sealed class SuggestionsSheetsService : EclipseSheetsService<SuggestionDto>, ISuggestionsSheetsService
{
    public SuggestionsSheetsService(
        IGoogleSheetsService service,
        IObjectParser<SuggestionDto> parser,
        IConfiguration configuration)
        : base(service, parser, configuration)
    {
    }

    protected override string Range => Configuration["Sheets:SuggestionsRange"]!;
}
