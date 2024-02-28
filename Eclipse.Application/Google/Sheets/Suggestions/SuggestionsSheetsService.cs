using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Common.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.Suggestions;

internal sealed class SuggestionsSheetsService : EclipseSheetsService<SuggestionSheetsModel>, ISuggestionsSheetsService
{
    public SuggestionsSheetsService(
        ISheetsService service,
        IObjectParser<SuggestionSheetsModel> parser,
        IConfiguration configuration)
        : base(service, parser, configuration)
    {
    }

    protected override string Range => Configuration["Sheets:SuggestionsRange"]!;
}
