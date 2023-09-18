using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets;

internal abstract class EclipseSheetsService<TObject> : IEclipseSheetsService<TObject>
{
    protected readonly IGoogleSheetsService Service;

    protected readonly IObjectParser<TObject> Parser;

    protected readonly IConfiguration Configuration;

    protected readonly string SheetId;

    protected abstract string Range { get; }

    public EclipseSheetsService(IGoogleSheetsService service, IObjectParser<TObject> parser, IConfiguration configuration)
    {
        Service = service;
        Parser = parser;
        Configuration = configuration;
        SheetId = Configuration["Sheets:SheetId"]!;
    }

    public virtual IReadOnlyList<TObject> GetAll()
    {
        return Service.Get(SheetId, Range, Parser).ToList();
    }

    public virtual void Add(TObject value)
    {
        Service.Append(SheetId, Range, value, Parser);
    }
}
