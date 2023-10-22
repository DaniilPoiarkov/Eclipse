using Eclipse.Infrastructure.Google.Sheets;

using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Internals.Google.Sheets;

internal class GoogleSheetsService : IGoogleSheetsService
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
        
        return values.Values.Skip(_namesRow).Select(parser.Parse);
    }

    public void Append<TObject>(string sheetId, string range, TObject value, IObjectParser<TObject> parser)
    {
        var values = parser.Parse(value);

        var valueRange = new ValueRange
        {
            Values = new List<IList<object>>() { values },
        };

        var appendRequest = _sheetsService.Spreadsheets.Values.Append(valueRange, sheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        appendRequest.Execute();
    }
}
