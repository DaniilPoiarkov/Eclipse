using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Google;

/// <summary>
/// Provides related to google services
/// </summary>
public interface IGoogleClient
{
    SheetsService GetSheetsService();
}
