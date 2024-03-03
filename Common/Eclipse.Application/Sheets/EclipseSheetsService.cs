using Eclipse.Common.EventBus;
using Eclipse.Common.Sheets;
using Eclipse.Domain.Behaviors;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Sheets;

public abstract class EclipseSheetsService<TObject> : IEclipseSheetsService<TObject>
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

    public virtual async Task<IReadOnlyList<TObject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return (await Service.GetAsync(SheetId, Range, Parser, cancellationToken)).ToList();
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
