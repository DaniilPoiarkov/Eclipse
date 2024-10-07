using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Reports;

internal static class MoodStateExtensions
{
    internal static int ToScore(this MoodState state)
    {
        return state switch
        {
            MoodState.Good => 5,
            MoodState.SlightlyGood => 4,
            MoodState.Neutral => 3,
            MoodState.SlightlyBad => 2,
            MoodState.Bad => 1,
            _ => 0,
        };
    }
}
