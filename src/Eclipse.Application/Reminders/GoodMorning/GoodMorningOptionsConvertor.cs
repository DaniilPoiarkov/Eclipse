using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningOptionsConvertor : IOptionsConvertor<User, SendGoodMorningSchedulerOptions>
{
    public SendGoodMorningSchedulerOptions Convert(User from)
    {
        return new SendGoodMorningSchedulerOptions(from.Id, from.Gmt);
    }
}
