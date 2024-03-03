namespace Eclipse.Application.Sheets;

public interface IEclipseSheetsService<TObject>
{
    Task<IReadOnlyList<TObject>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(TObject value, CancellationToken cancellationToken = default);
}
