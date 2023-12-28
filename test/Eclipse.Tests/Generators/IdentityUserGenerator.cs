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
            var user = IdentityUser.Create(
                Guid.NewGuid(),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                chatId: i);

            result.Add(user);
        }

        return result;
    }
}
