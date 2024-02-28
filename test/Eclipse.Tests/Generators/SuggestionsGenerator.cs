using Bogus;

using Eclipse.Domain.Suggestions;

namespace Eclipse.Tests.Generators;

public static class SuggestionsGenerator
{
    public static List<Suggestion> Generate(int count, long baseUserId)
    {
        var suggestions = new List<Suggestion>(count);

        var faker = new Faker();

        for (int i = 0; i < count; i++)
        {
            suggestions.Add(new Suggestion(Guid.NewGuid(), faker.Lorem.Word(), baseUserId++, faker.Date.Past(1, DateTime.UtcNow)));
        }

        return suggestions;
    }
}
