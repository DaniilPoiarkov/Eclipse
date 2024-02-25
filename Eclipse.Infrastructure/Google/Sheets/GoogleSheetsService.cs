using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Eclipse.Common.Sheets;

namespace Eclipse.Infrastructure.Google.Sheets;

internal sealed class GoogleSheetsService : ISheetsService
{
    private static readonly int _namesRow = 1;

    private readonly SheetsService _sheetsService;

    public GoogleSheetsService(SheetsService sheetsService)
    {
        _sheetsService = sheetsService;
    }

    public IEnumerable<TObject> Get<TObject>(string sheetId, string range, IObjectParser<TObject> parser)
    {
        var request = _sheetsService.Spreadsheets.Values.Get(sheetId, range);
        var values = request.Execute();

        return values.Values
            .Skip(_namesRow)
            .Select(parser.Parse)
            .Select(result => result.Value);
    }

    public void Append<TObject>(string sheetId, string range, TObject value, IObjectParser<TObject> parser)
    {
        var values = parser.Parse(value);

        var valueRange = new ValueRange
        {
            Values = [values],
        };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        appendRequest.Execute();
    }
}
