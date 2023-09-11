using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Google;

public interface IGoogleClient
{
    SheetsService GetSheetsService();
}
