using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.MoodReport;

internal sealed class MoodReportOptionsConvertor : IOptionsConvertor<User, MoodReportSchedulerOptions>
{
    public MoodReportSchedulerOptions Convert(User from)
    {
        return new MoodReportSchedulerOptions(from.Id, from.Gmt);
    }
}
