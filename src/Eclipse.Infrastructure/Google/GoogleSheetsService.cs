using Eclipse.Common.Sheets;

using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Eclipse.Infrastructure.Google;

internal sealed class GoogleSheetsService : ISheetsService
{
    private static readonly int _namesRow = 1;

    private readonly SheetsService _sheetsService;

    public GoogleSheetsService(SheetsService sheetsService)
    {
        _sheetsService = sheetsService;
    }

    public async Task<IEnumerable<TObject>> GetAsync<TObject>(string sheetId, string range, IObjectParser<TObject> parser, CancellationToken cancellationToken = default)
    {
        var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var values = await request.ExecuteAsync(cancellationToken);

        return values.Values
            .Skip(_namesRow)
            .Select(parser.Parse)
            .Where(r => r.IsSuccess)
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
