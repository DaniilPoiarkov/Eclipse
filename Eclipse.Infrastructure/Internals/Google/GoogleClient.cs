using Eclipse.Infrastructure.Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Internals.Google;

internal class GoogleClient : IGoogleClient
{
    private readonly string _credentials;

    public GoogleClient(string credentials)
    {
        _credentials = credentials;
    }

    public SheetsService GetSheetsService()
    {
        var baseClient = InitializeBaseClient(SheetsService.Scope.Spreadsheets);
        return new SheetsService(baseClient);
    }

    private BaseClientService.Initializer InitializeBaseClient(string scope)
    {
        var credentials = GoogleCredential.FromJson(_credentials)
            .CreateScoped(scope);

        return new BaseClientService.Initializer()
        {
            HttpClientInitializer = credentials
        };
    }
}
