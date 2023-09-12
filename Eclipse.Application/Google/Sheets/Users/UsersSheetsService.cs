using Eclipse.Application.Contracts.Google.Sheets.Users;
using Eclipse.Core.Models;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.Users;

internal class UsersSheetsService : EclipseSheetsService<TelegramUser>, IUsersSheetsService
{
    private readonly IConfiguration _configuration;

    public UsersSheetsService(
        IGoogleSheetsService service,
        IObjectParser<TelegramUser> parser,
        IConfiguration configuration)
        : base(service, parser)
    {
        _configuration = configuration;
    }

    protected override string Range => _configuration["Sheets:UsersRange"]!;

    protected override string SheetId => _configuration["Sheets:SheetId"]!;
}
