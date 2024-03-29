﻿using Bogus;

using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Tests.Generators;

public static class IdentityUserDtoGenerator
{
    public static List<IdentityUserDto> Generate(long baseId, int count)
    {
        return new Faker<IdentityUserDto>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.ChatId, _ => baseId++)
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .Generate(count);
    }

    public static List<IdentityUserSlimDto> GenerateSlim(long baseId, int count)
    {
        return new Faker<IdentityUserSlimDto>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.ChatId, _ => baseId++)
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .Generate(count);
    }
}
