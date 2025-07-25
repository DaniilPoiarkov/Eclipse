﻿using Bogus;

using Eclipse.Core.Context;

namespace Eclipse.Tests.Generators;

public class MessageContextGenerator
{
    public static MessageContext Generate(string value, IServiceProvider serviceProvider)
    {
        var person = new Faker().Person;

        var user = new TelegramUser(1, person.FirstName, person.LastName, person.UserName);
        return new MessageContext(1, value, user, serviceProvider);
    }
}
