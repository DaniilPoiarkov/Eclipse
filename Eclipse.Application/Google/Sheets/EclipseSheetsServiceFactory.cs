using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets;

internal class EclipseSheetsServiceFactory
{
    private readonly IGoogleSheetsService _googleSheetsService;

    private readonly IConfiguration _configuration;

    public EclipseSheetsServiceFactory(IGoogleSheetsService googleSheetsService, IConfiguration configuration)
    {
        _googleSheetsService = googleSheetsService;
        _configuration = configuration;
    }

    internal IEclipseSheetsService Build()
    {
        var sheetsId = _configuration["Sheets:SheetsId"];
        ArgumentNullException.ThrowIfNull(sheetsId, nameof(sheetsId));

        var suggestionsRange = _configuration["Sheets:SuggestionsRange"];
        ArgumentNullException.ThrowIfNull(suggestionsRange, nameof(suggestionsRange));
        
        var usersRange = _configuration["Sheets:UsersRange"];
        ArgumentNullException.ThrowIfNull(usersRange, nameof(usersRange));

        return new EclipseSheetsService(_googleSheetsService, sheetsId, suggestionsRange, usersRange);
    }
}
