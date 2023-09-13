using Eclipse.Application.Contracts.Google.Sheets.Users;
using Eclipse.Core.Models;
using Eclipse.Infrastructure.Google.Sheets;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.Google.Sheets.Users;

internal class UsersSheetsService : EclipseSheetsService<TelegramUser>, IUsersSheetsService
{    
    public UsersSheetsService(
        IGoogleSheetsService service,
        IObjectParser<TelegramUser> parser,
        IConfiguration configuration)
        : base(service, parser, configuration)
    {
    }

    protected override string Range => Configuration["Sheets:UsersRange"]!;
}
