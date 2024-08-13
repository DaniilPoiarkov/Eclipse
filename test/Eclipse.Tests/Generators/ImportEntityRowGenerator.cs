using Bogus;

using Eclipse.Application.Exporting.Reminders;
using Eclipse.Application.Exporting.TodoItems;
using Eclipse.Application.Exporting.Users;

namespace Eclipse.Tests.Generators;

public static class ImportEntityRowGenerator
{
    public static ImportTodoItemDto TodoItem()
    {
        var faker = new Faker();

        return new ImportTodoItemDto
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),

            Text = faker.Lorem.Word(),
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static ImportReminderDto Reminder(string notifyAt = "12:00:00")
    {
        var faker = new Faker();

        return new ImportReminderDto
        {
            Id = Guid.NewGuid(),
            NotifyAt = notifyAt,
            Text = faker.Lorem.Word(),
            UserId = Guid.NewGuid()
        };
    }

    public static ImportUserDto User(string culture = "en", string gmt = "0")
    {
        var faker = new Faker();

        return new ImportUserDto
        {
            Id = Guid.NewGuid(),

            Name = faker.Person.FirstName,
            Surname = faker.Person.LastName,
            UserName = faker.Person.UserName,

            ChatId = faker.Random.Long(min: 1),
            Culture = culture,
            Gmt = gmt,
            NotificationsEnabled = faker.Random.Bool()
        };
    }
}
