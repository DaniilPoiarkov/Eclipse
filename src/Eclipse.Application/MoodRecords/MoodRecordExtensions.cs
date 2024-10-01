using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Domain.MoodRecords;

namespace Eclipse.Application.MoodRecords;

internal static class MoodRecordExtensions
{
    internal static MoodRecordDto ToDto(this MoodRecord record)
    {
        return new MoodRecordDto
        {
            Id = record.Id,
            UserId = record.UserId,
            State = record.State,
            CreatedAt = record.CreatedAt,
        };
    }
}
