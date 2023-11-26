using Bogus;

using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Tests.Generators;

public static class IdentityUserGenerator
{
    public static List<IdentityUser> Generate(int count)
    {
        var result = new List<IdentityUser>(count);

        var faker = new Faker();

        for (int i = 0; i < count; i++)
        {
            result.Add(new IdentityUser(
                Guid.NewGuid(),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                chatId: i,
                culture: "en",
                notificationsEnabled: i % 2 == 0));
        }

        return result;
    }
}
