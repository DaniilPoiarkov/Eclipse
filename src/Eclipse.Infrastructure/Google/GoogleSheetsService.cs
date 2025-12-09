using Eclipse.Common.Sheets;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

using Microsoft.Extensions.Logging;

namespace Eclipse.Infrastructure.Google;

internal sealed class GoogleSheetsService : ISheetsService
{
    private static readonly int _namesRow = 1;

    private readonly SheetsService _sheetsService;

    private readonly ILogger<GoogleSheetsService> _logger;

    public GoogleSheetsService(SheetsService sheetsService, ILogger<GoogleSheetsService> logger)
    {
        _sheetsService = sheetsService;
        _logger = logger;
    }

    public async Task<IEnumerable<TObject>> GetAsync<TObject>(string sheetId, string range, IObjectParser<TObject> parser, CancellationToken cancellationToken = default)
    {
        var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var values = await request.ExecuteAsync(cancellationToken);

        var parsed = values.Values
            .Skip(_namesRow)
            .Select(parser.Parse);

        var errors = parsed.Where(r => !r.IsSuccess)
            .Select(r => r.Error);

        foreach (var error in errors)
        {
            _logger.LogWarning("Failed to parse row from Google Sheets: {Error}", error);
        }

        return parsed.Where(r => r.IsSuccess)
            .Select(result => result.Value);
    }

    public Task AppendAsync<TObject>(string sheetId, string range, TObject value, IObjectParser<TObject> parser, CancellationToken cancellationToken = default)
    {
        var values = parser.Parse(value);

        var valueRange = new ValueRange
        {
            Values = [values],
        };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        return appendRequest.ExecuteAsync(cancellationToken);
    }
}
