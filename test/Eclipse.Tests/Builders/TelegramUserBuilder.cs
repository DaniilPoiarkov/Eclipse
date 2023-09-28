using Bogus;

using Eclipse.Core.Models;

namespace Eclipse.Tests.Builders;

public static class TelegramUserBuilder
{
    public static List<TelegramUser> GenerateUsers(long baseId, int count)
    {
        return new Faker<TelegramUser>()
            .RuleFor(u => u.Id, _ => baseId++)
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .Generate(count);
    }
}
