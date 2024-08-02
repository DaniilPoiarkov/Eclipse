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
                chatId: i);

            user.Culture = i % 2 == 0 ? "en" : "uk";

            result.Add(user);
        }

        return result;
    }

    public static User Get()
    {
        return Generate(1).First();
    }
}
