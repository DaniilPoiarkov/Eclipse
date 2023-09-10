using Google.Apis.Sheets.v4;

namespace Eclipse.Infrastructure.Sheets;

public interface IGoogleClient
{
    SheetsService GetSheetsService();
}
