using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Pipelines.Pipelines.Daily.MoodRecords;

internal sealed record MoodAnswer(
    MoodState? State,
    string Message
);
