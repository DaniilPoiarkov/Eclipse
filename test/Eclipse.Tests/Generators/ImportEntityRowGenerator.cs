using Bogus;

using Eclipse.Application.Exporting.Reminders;
using Eclipse.Application.Exporting.TodoItems;

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
}
