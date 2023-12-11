namespace Eclipse.Tests.Bdd.Application.Extensions.StepDefinitions;

[Binding]
internal sealed class TryParseAsTimeOnlyStepDefinitions
{
    private string? DefinedString { get; set; }

    private readonly ScenarioContext _scenarioContext;


    private static readonly string _booleanKey = "boolean";

    private static readonly string _timeOnlyKey = "time-only";

    public TryParseAsTimeOnlyStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("Following string: (.*)")]
    public void GivenFollowingString(string value) => DefinedString = value;

    [When("Call TryParseAsTimeOnly extension method")]
    public void WhenCallMethod()
    {
        TimeOnly timeOnly = default;

        var isSucceded = DefinedString?.TryParseAsTimeOnly(out timeOnly);

        _scenarioContext.Add(_booleanKey, isSucceded);
        _scenarioContext.Add(_timeOnlyKey, timeOnly);
    }

    [Then("Bollean indicates as (.*) and outcome value must contain (.*) hours and (.*) minutes")]
    public void ThenIsSuccededMustBeAsExpectedAsWellAsTime(bool isSucceded, int hours, int minutes)
    {
        var parsingResult = _scenarioContext.Get<bool>(_booleanKey);
        var timeResult = _scenarioContext.Get<TimeOnly>(_timeOnlyKey);

        parsingResult.Should().Be(isSucceded);
        timeResult.Hour.Should().Be(hours);
        timeResult.Minute.Should().Be(minutes);
    }
}
