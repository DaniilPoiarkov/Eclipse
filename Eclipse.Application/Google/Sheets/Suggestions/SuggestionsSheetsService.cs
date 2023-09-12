using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.Suggestions;

internal class SuggestionsSheetsService : EclipseSheetsService<SuggestionDto>, ISuggestionsSheetsService
{
    private readonly IConfiguration _configuration;

    public SuggestionsSheetsService(
        IGoogleSheetsService service,
        IObjectParser<SuggestionDto> parser,
        IConfiguration configuration)
        : base(service, parser)
    {
        _configuration = configuration;
    }

    protected override string Range => _configuration["Sheets:SuggestionsRange"]!;

    protected override string SheetId => _configuration["Sheets:SheetId"]!;
}
