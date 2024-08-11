using Bogus;

using Eclipse.Domain.Users;

namespace Eclipse.Tests.Generators;

public static class UserGenerator
{
    public static List<User> Generate(int count)
    {
        var result = new List<User>(count);

        var faker = new Faker();

        for (int i = 0; i < count; i++)
        {
            var user = User.Create(
                Guid.NewGuid(),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                chatId: i,
                newRegistered: i % 2 == 0);

            user.Culture = i % 2 == 0 ? "en" : "uk";

            result.Add(user);
        }

        return result;
    }

    public static User Get(long chatId = default)
    {
        var faker = new Faker();

        var user = User.Create(
            Guid.NewGuid(),
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.UserName,
            chatId,
            true
        );

        user.Culture = Random.Shared.Next(0, 10) % 2 == 0
            ? "en"
            : "uk";

        return user;
    }
}
