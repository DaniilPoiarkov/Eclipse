using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Sheets;

internal class GoogleClient : IGoogleClient
{
    private readonly string _apiKey;

    public GoogleClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public SheetsService GetSheetsService()
    {
        var baseClient = InitializeBaseClient(SheetsService.Scope.Spreadsheets);
        return new SheetsService(baseClient);
    }

    private BaseClientService.Initializer InitializeBaseClient(string scope)
    {
        var creds = GoogleCredential.FromAccessToken(_apiKey)
            .CreateScoped(scope);

        return new BaseClientService.Initializer()
        {
            HttpClientInitializer = creds
        };
    }
}
