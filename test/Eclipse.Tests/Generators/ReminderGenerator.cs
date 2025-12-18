using Bogus;

using Eclipse.Domain.Reminders;

namespace Eclipse.Tests.Generators;

public class ReminderGenerator
{
    public static List<Reminder> Generate(int count, TimeOnly time = default)
    {
        var faker = new Faker();

        var reminders = new List<Reminder>(count);

        for (int i = 0; i < count; i++)
        {
            reminders.Add(new Reminder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), time));
        }

        return reminders;
    }
}
