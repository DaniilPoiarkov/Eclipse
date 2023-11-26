using Bogus;

using Eclipse.Domain.TodoItems;

namespace Eclipse.Tests.Generators;

public static class TodoItemsGenerator
{
    public static List<TodoItem> Generate(Guid userId, int count)
    {
        var results = new List<TodoItem>(count);
        var faker = new Faker();

        for (int i = 0; i < count; i++)
        {
            results.Add(new TodoItem(Guid.NewGuid(), userId, faker.Lorem.Word(), faker.Date.Past()));
        }

        return results;
    }
}
