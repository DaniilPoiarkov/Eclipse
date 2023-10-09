using Eclipse.Domain.Reminders;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.Reminders;

public class ReminderNotifyAtSpecificationTests
{
    [Fact]
    public void IsSatisfied_WhenSpecified_ThenFiltersDataProperly()
    {
        var time = TimeOnly.FromDateTime(DateTime.UtcNow);

        var reminders = ReminderGenerator.Generate(5, time);
        var futureReminders = ReminderGenerator.Generate(5, time.AddHours(1));

        reminders.AddRange(futureReminders);

        var specification = new ReminderNotifyAtSpecification(time);

        var results = reminders.Where(specification).ToList();

        results.Count.Should().Be(5);
        results.All(r => r.NotifyAt == time).Should().BeTrue();
    }
}
