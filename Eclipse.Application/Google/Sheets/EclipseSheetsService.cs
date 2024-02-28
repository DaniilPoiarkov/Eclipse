using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Domain.Shared.Entities;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets;

internal abstract class EclipseSheetsService<TObject> : IEclipseSheetsService<TObject>
{
    protected readonly ISheetsService Service;

    protected readonly IObjectParser<TObject> Parser;

    protected readonly IConfiguration Configuration;

    protected readonly IEventBus EventBus;

    protected readonly string SheetId;

    protected abstract string Range { get; }

    public EclipseSheetsService(ISheetsService service, IObjectParser<TObject> parser, IConfiguration configuration, IEventBus eventBus)
    {
        Service = service;
        Parser = parser;
        Configuration = configuration;
        SheetId = Configuration["Sheets:SheetId"]!;
        EventBus = eventBus;
    }

    public virtual IReadOnlyList<TObject> GetAll()
    {
        return Service.Get(SheetId, Range, Parser).ToList();
    }

    public virtual async Task AddAsync(TObject value, CancellationToken cancellationToken = default)
    {
        if (value is IHasDomainEvents hasDomainEvents)
        {
            foreach (var @event in hasDomainEvents.GetEvents())
            {
                await EventBus.Publish(@event, cancellationToken);
            }
        }

        await Service.AppendAsync(SheetId, Range, value, Parser, cancellationToken);
    }
}
