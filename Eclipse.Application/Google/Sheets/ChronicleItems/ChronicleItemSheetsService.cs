using Eclipse.Application.Contracts.ChronicleItems;
using Eclipse.Application.Contracts.Google.Sheets.ChronicleItems;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.ChronicleItems;

internal class ChronicleItemSheetsService : EclipseSheetsService<ChronicleItemDto>, IChronicleItemSheetsService
{
    public ChronicleItemSheetsService(
        IGoogleSheetsService service,
        IObjectParser<ChronicleItemDto> parser,
        IConfiguration configuration)
        : base(service, parser, configuration)
    {
    }

    protected override string Range => Configuration["Sheets:ChronicleItemsRange"]!;
}
