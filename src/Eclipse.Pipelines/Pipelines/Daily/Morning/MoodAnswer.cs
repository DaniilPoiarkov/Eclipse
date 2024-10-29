using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Pipelines.Pipelines.Daily.Morning;

internal sealed record MoodAnswer(
    MoodState? State,
    string Message
);
