using Bogus;

using Eclipse.Application.Contracts.Suggestions;

namespace Eclipse.Tests.Generators;

public static class SuggestionsGenerator
{
    public static List<SuggestionSheetsModel> Generate(int count, long baseUserId)
    {
        return new Faker<SuggestionSheetsModel>()
            .RuleFor(s => s.Id, _ => Guid.NewGuid())
            .RuleFor(s => s.Text, f => f.Lorem.Word())
            .RuleFor(s => s.TelegramUserId, baseUserId++)
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow))
            .Generate(count);
    }
}
