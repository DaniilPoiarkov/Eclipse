using Eclipse.Common.Clock;
using Eclipse.Common.Sheets;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Suggestions;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets;

internal sealed class SuggestionsSheetsService : EclipseSheetsService<Suggestion>
{
    public SuggestionsSheetsService(
        ISheetsService service,
        IObjectParser<Suggestion> parser,
        IConfiguration configuration,
        IOutboxMessageRepository outboxMessageRepository,
        ITimeProvider timeProvider)
        : base(service, parser, configuration, outboxMessageRepository, timeProvider) { }

    protected override string Range => Configuration["Sheets:SuggestionsRange"]
        ?? throw new InvalidDataException("Suggestions range not provided.");
}
