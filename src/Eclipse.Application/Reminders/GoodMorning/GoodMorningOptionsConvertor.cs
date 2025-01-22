using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningOptionsConvertor : IOptionsConvertor<User, GoodMorningSchedulerOptions>
{
    public GoodMorningSchedulerOptions Convert(User from)
    {
        return new GoodMorningSchedulerOptions(from.Id, from.Gmt);
    }
}
