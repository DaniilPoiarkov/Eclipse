using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.MoodRecords;

public static class MoodStateExtensions
{
    public static int ToScore(this MoodState state)
    {
        return state switch
        {
            MoodState.Amazing => 10,
            MoodState.Excellent => 9,
            MoodState.VeryGood => 8,
            MoodState.Good => 7,
            MoodState.Fine => 6,
            MoodState.Neutral => 5,
            MoodState.Poor => 4,
            MoodState.Bad => 3,
            MoodState.VeryBad => 2,
            MoodState.Terrible => 1,
            _ => 0,
        };
    }
}
