using Eclipse.Domain.Users;

namespace Eclipse.Tests.Bdd.Domain.Users.StepDefinitions;

[Binding]
public sealed class AddReminderStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    private readonly User _user;

    private static readonly string _reminderTextKey = "reminder-text";

    public AddReminderStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _user = User.Create(Guid.NewGuid(), "Name", "Surname", "Username", 1);
    }

    [Given("Existing user with (.*) reminders")]
    public void GivenUserWithRemindersCount(int reminders)
    {
        var time = TimeOnly.FromDateTime(DateTime.UtcNow)
            .AddHours(1);

        for (int i = 0; i < reminders; i++)
        {
            _user.AddReminder($"Test {i + 1}", time);
        }
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

        var text = _scenarioContext.Get<string>(_reminderTextKey);

        _user.AddReminder(text, timeOnly);
    }

    [Then("User must have (.*) reminders")]
    public void ThenUserMastHaveReminders(int remindersCount)
    {
        _user.Reminders.Count.Should().Be(remindersCount);
    }
}
