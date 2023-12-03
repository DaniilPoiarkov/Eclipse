using Eclipse.Domain.IdentityUsers;

namespace Eclipse.Tests.Bdd.Domain.IdentityUsers.StepDefinitions;

[Binding]
public sealed class AddReminderStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    private static readonly string _userKey = "user";

    private static readonly string _reminderTextKey = "reminder-text";

    private static readonly string _reminderTimeKey = "reminder-time";

    public AddReminderStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("Existing user with (.*) reminders")]
    public void GivenUserWithRemindersCount(int reminders)
    {
        var user = IdentityUser.Create(Guid.NewGuid(), "Name", "Surname", "Username", 1);

        var time = TimeOnly.FromDateTime(DateTime.UtcNow)
            .AddHours(1);

        for (int i = 0; i < reminders; i++)
        {
            user.AddReminder($"Test {i + 1}", time);
        }

        _scenarioContext.Add(_userKey, user);
    }

    [When("Add reminder with following text: \"(.*)\"")]
    public void WhenAddReminderWithText(string text)
    {
        _scenarioContext.Add(_reminderTextKey, text);
    }

    [When("Following time: (.*)")]
    public void AndFollowingTime(string time)
    {
        _ = time.TryParseAsTimeOnly(out var timeOnly);

        _scenarioContext.Add(_reminderTimeKey, timeOnly);
    }

    [Then("User must have (.*) reminders")]
    public void ThenReminderShouldBeCreated(int remindersCount)
    {
        var user = _scenarioContext.Get<IdentityUser>(_userKey);
        var text = _scenarioContext.Get<string>(_reminderTextKey);
        var time = _scenarioContext.Get<TimeOnly>(_reminderTimeKey);

        user.AddReminder(text, time);

        user.Reminders.Count.Should().Be(remindersCount);
    }
}
