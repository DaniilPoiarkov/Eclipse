using Eclipse.Domain.Users;

using TechTalk.SpecFlow.Assist;

namespace Eclipse.Tests.Bdd.Domain.Users.StepDefinitions;

[Binding]
public sealed class AddTodoItemStepDefinitions
{
    private readonly User _user = User.Create(Guid.NewGuid(), "Name", "Surname", "Username", 1, true);

    [Given("User with todo items")]
    public void GivenUserWithTodoItems(Table table)
    {
        var providedTodoItems = table.CreateSet<CreateTodoItem>();

        foreach (var item in providedTodoItems)
        {
            _user.AddTodoItem(item.Text, DateTime.UtcNow);
        }
    }

    [When("Add todo item with following text: \"(.*)\"")]
    public void WhenAddTodoItemWithText(string text)
    {
        _user.AddTodoItem(text, DateTime.UtcNow);
    }

    [Then("User must have (.*) todo items")]
    public void Then(int itemsCount)
    {
        _user.TodoItems.Count.Should().Be(itemsCount);
    }
}

file class CreateTodoItem
{
    public string? Text { get; set; }
}
