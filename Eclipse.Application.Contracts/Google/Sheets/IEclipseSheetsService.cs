namespace Eclipse.Application.Contracts.Google.Sheets;

public interface IEclipseSheetsService<TObject>
{
    IReadOnlyList<TObject> GetAll();

    void Add(TObject value);
}
