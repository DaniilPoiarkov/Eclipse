namespace Eclipse.Application.Contracts.Google.Sheets;

public interface IEclipseSheetsService<TObject>
{
    IReadOnlyList<TObject> GetAll();

    Task AddAsync(TObject value, CancellationToken cancellationToken = default);
}
