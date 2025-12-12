using Eclipse.Common.Clock;
using Eclipse.Common.Sheets;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Shared.Entities;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace Eclipse.Application.Google.Sheets;

internal abstract class EclipseSheetsService<TObject> : IEclipseSheetsService<TObject>
{
    protected readonly ISheetsService Service;

    protected readonly IObjectParser<TObject> Parser;

    protected readonly IConfiguration Configuration;

    protected readonly IOutboxMessageRepository OutboxMessageRepository;

    protected readonly ITimeProvider TimeProvider;

    protected readonly string SheetId;

    protected abstract string Range { get; }

    public EclipseSheetsService(
        ISheetsService service,
        IObjectParser<TObject> parser,
        IConfiguration configuration,
        IOutboxMessageRepository outboxMessageRepository,
        ITimeProvider timeProvider)
    {
        Service = service;
        Parser = parser;
        Configuration = configuration;
        SheetId = Configuration["Sheets:SheetId"]!;
        OutboxMessageRepository = outboxMessageRepository;
        TimeProvider = timeProvider;
    }

    public virtual Task<IEnumerable<TObject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Service.GetAsync(SheetId, Range, Parser, cancellationToken);
    }

    public virtual async Task AddAsync(TObject value, CancellationToken cancellationToken = default)
    {
        if (value is IHasDomainEvents hasDomainEvents)
        {
            var messages = hasDomainEvents.GetEvents()
                .Select(@event => new OutboxMessage(
                    Guid.CreateVersion7(),
                    @event.GetType().AssemblyQualifiedName!,
                    JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    }),
                    TimeProvider.Now
                ));

            await OutboxMessageRepository.CreateRangeAsync(messages, cancellationToken);
        }

        await Service.AppendAsync(SheetId, Range, value, Parser, cancellationToken);
    }
}
