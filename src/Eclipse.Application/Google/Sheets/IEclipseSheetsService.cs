namespace Eclipse.Application.Google.Sheets;

internal interface IEclipseSheetsService<TObject>
{
    Task<IEnumerable<TObject>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TObject value, CancellationToken cancellationToken = default);
}
