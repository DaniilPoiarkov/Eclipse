namespace Eclipse.Common.Sheets;

public interface ISheetsService
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject">Model which represents fetched value</typeparam>
    /// <param name="sheetId">SpreadSHeet Id</param>
    /// <param name="range">List name and range of columns e.g. List1!B:J</param>
    /// <param name="parser">Implementation which can parse list of objects to desired model</param>
    /// <returns></returns>
    Task<IEnumerable<TObject>> GetAsync<TObject>(string sheetId, string range, IObjectParser<TObject> parser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="IObject">Model which will be parsed and added to spreadsheet</typeparam>
    /// <param name="sheetId">SpreadSHeet Id</param>
    /// <param name="range">List name and range of columns e.g. List1!B:J</param>
    /// <param name="value">Exact value which will be parsed and added to spreadsheets</param>
    /// <param name="parser">Implementation which can parse object to list of objects</param>
    Task AppendAsync<IObject>(string sheetId, string range, IObject value, IObjectParser<IObject> parser, CancellationToken cancellationToken = default);
}
