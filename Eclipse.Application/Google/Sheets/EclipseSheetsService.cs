using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Infrastructure.Google.Sheets;

namespace Eclipse.Application.Google.Sheets;

internal abstract class EclipseSheetsService<TObject> : IEclipseSheetsService<TObject>
{
    private readonly IGoogleSheetsService _service;

    private readonly IObjectParser<TObject> _parser;

    protected abstract string Range { get; }

    protected abstract string SheetId { get; }

    public EclipseSheetsService(IGoogleSheetsService service, IObjectParser<TObject> parser)
    {
        _service = service;
        _parser = parser;
    }

    public IReadOnlyList<TObject> GetAll()
    {
        return _service.Get(SheetId, Range, _parser).ToList();
    }

    public void Add(TObject value)
    {
        _service.Append(SheetId, Range, value, _parser);
    }
}
