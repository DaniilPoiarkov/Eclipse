using Eclipse.Domain.IdentityUsers;

using TechTalk.SpecFlow.Assist;

namespace Eclipse.Tests.Bdd.Domain.IdentityUsers.StepDefinitions;

[Binding]
public sealed class AddTodoItemStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;

    private static readonly string _userKey = "user";

    public AddTodoItemStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("User with todo items")]
    public void GivenUserWithTodoItems(Table table)
    {
        var user = IdentityUser.Create(Guid.NewGuid(), "Name", "Surname", "Username", 1);
        
        var providedTodoItems = table.CreateSet<CreateTodoItem>();

        foreach (var item in providedTodoItems)
        {
            user.AddTodoItem(item.Text);
        }

        _scenarioContext.Add(_userKey, user);
    }

    [When("Add todo item with following text: \"(.*)\"")]
    public void WhenAddTodoItemWithText(string text)
    {
        var user = _scenarioContext.Get<IdentityUser>(_userKey);

        user.AddTodoItem(text);
    }

    [Then("User must have (.*) todo items")]
    public void Then(int itemsCount)
    {
        var user = _scenarioContext.Get<IdentityUser>(_userKey);
        
        user.TodoItems.Count.Should().Be(itemsCount);
    }
}

file class CreateTodoItem
{
    public string? Text { get; set;}
}
