using Eclipse.Infrastructure.Builder;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

using Microsoft.Extensions.Options;

namespace Eclipse.Infrastructure.Google;

internal sealed class GoogleClient : IGoogleClient
{
    private readonly IOptions<GoogleOptions> _options;

    public GoogleClient(IOptions<GoogleOptions> options)
    {
        _options = options;
    }

    public SheetsService GetSheetsService()
    {
        var baseClient = InitializeBaseClient(SheetsService.Scope.Spreadsheets);
        return new SheetsService(baseClient);
    }

    private BaseClientService.Initializer InitializeBaseClient(string scope)
    {
        var credentials = GoogleCredential.FromJson(_options.Value.Credentials)
            .CreateScoped(scope);

        return new BaseClientService.Initializer()
        {
            HttpClientInitializer = credentials
        };
    }
}
