namespace Eclipse.Application.Contracts.Configuration;

public interface IConfigurationService
{
    List<CultureInfo> GetCultures();

    List<MoodStateScoreDto> GetMoodStateScores();
}
