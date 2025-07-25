﻿using Bogus;

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
                DateTime.UtcNow,
                true,
                newRegistered: i % 2 == 0
            );

            user.Culture = i % 2 == 0 ? "en" : "uk";

            result.Add(user);
        }

        return result;
    }

    public static User Get(long chatId = default, bool newRegistered = true)
    {
        var faker = new Faker();

        var user = User.Create(
            Guid.NewGuid(),
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.UserName,
            chatId,
            DateTime.UtcNow,
            true,
            newRegistered
        );

        user.Culture = Random.Shared.Next(0, 10) % 2 == 0
            ? "en"
            : "uk";

        return user;
    }

    public static IEnumerable<User> GetWithIds(IEnumerable<Guid> userIds)
    {
        var faker = new Faker();

        return userIds.Select(id =>
            User.Create(id,
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Random.Long(min: 1),
                DateTime.UtcNow,
                true,
                false
            )
        );
    }
}
