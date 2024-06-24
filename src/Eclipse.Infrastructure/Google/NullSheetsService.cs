using Eclipse.Common.Sheets;

namespace Eclipse.Infrastructure.Google;

internal sealed class NullSheetsService : ISheetsService
{
    public Task AppendAsync<IObject>(string sheetId, string range, IObject value, IObjectParser<IObject> parser, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TObject>> GetAsync<TObject>(string sheetId, string range, IObjectParser<TObject> parser, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<TObject>>([]);
    }
}
