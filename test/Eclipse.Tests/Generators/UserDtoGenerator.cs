using Bogus;

using Eclipse.Application.Contracts.Users;

namespace Eclipse.Tests.Generators;

public static class UserDtoGenerator
{
    public static List<UserDto> Generate(long baseId, int count)
    {
        return new Faker<UserDto>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.ChatId, _ => baseId++)
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .Generate(count);
    }

    public static List<UserSlimDto> GenerateSlim(long baseId, int count)
    {
        return new Faker<UserSlimDto>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.ChatId, _ => baseId++)
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .Generate(count);
    }
}
