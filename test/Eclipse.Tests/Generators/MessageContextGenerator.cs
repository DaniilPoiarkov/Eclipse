using Bogus;

using Eclipse.Core.Core;
using Eclipse.Core.Models;

namespace Eclipse.Tests.Generators;

public class MessageContextGenerator
{
    public static MessageContext Generate(string value)
    {
        var person = new Faker().Person;

        var user = new TelegramUser(1, person.FirstName, person.LastName, person.UserName);
        return new MessageContext(1, value, user);
    }
}
