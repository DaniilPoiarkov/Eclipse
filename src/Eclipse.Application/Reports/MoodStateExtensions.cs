using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Reports;

public static class MoodStateExtensions
{
    public static int ToScore(this MoodState state)
    {
        return state switch
        {
            MoodState.Excelent => 9,
            MoodState.AlmsotExcelent => 8,
            MoodState.Good => 7,
            MoodState.SlightlyGood => 6,
            MoodState.Neutral => 5,
            MoodState.SlightlyBad => 4,
            MoodState.Bad => 3,
            MoodState.AlmsotWorst => 2,
            MoodState.Worst => 1,
            _ => 0,
        };
    }
}
