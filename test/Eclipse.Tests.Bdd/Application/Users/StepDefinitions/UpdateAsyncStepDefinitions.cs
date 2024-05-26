using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Domain.Users;

using NSubstitute;

namespace Eclipse.Tests.Bdd.Application.Users.StepDefinitions;

[Binding]
internal sealed class UpdateAsyncStepDefinitions
{
    private readonly Lazy<UserManager> _lazyManager;

    private readonly IUserRepository _repository;

    private readonly UserUpdateDto _updateDto = new();

    private readonly Lazy<IUserCreateUpdateService> _lazySut;

    private IUserCreateUpdateService Sut => _lazySut.Value;

    private UserDto _result = new();

    private User _user = default!;

    public UpdateAsyncStepDefinitions()
    {
        _repository = Substitute.For<IUserRepository>();

        _lazyManager = new Lazy<UserManager>(() => new UserManager(_repository));

        _lazySut = new Lazy<IUserCreateUpdateService>(
            () => new UserCreateUpdateService(_lazyManager.Value)
        );
    }

    [Given("An existing user with followind data:")]
    public void GivenAUserWithId(Table table)
    {
        _user = User.Create(
            Guid.Parse(table.Rows[0]["Id"]),
            table.Rows[0]["Name"],
            table.Rows[0]["Surname"],
            table.Rows[0]["Username"],
            long.Parse(table.Rows[0]["ChatId"]));

        _repository.FindAsync(_user.Id)!.ReturnsForAnyArgs(Task.FromResult(_user));
        _repository.UpdateAsync(_user)!.ReturnsForAnyArgs(Task.FromResult(_user));
    }

    [When("I update the user with the following details:")]
    public async Task WhenIUpdateTheUserWithTheFollowingDetails(Table table)
    {
        _updateDto.Name = table.Rows[0]["Name"];
        _updateDto.Surname = table.Rows[0]["Surname"];
        _updateDto.UserName = table.Rows[0]["Username"];
        _updateDto.Culture = table.Rows[0]["Culture"];
        _updateDto.NotificationsEnabled = bool.Parse(table.Rows[0]["NotificationsEnabled"]);

        _result = await Sut.UpdateAsync(_user.Id, _updateDto);
    }

    [Then("The user details should be updated successfully")]
    public void ThenTheUserDetailsShouldBeUpdatedSuccessfully()
    {
        _result.Should().NotBeNull();
    }

    [Then(@"The user's name should be ""(.*)""")]
    public void ThenTheUserNameShouldBe(string expectedName)
    {
        _result.Name.Should().Be(expectedName);
    }

    [Then(@"The user's surname should be ""(.*)""")]
    public void ThenTheUserSurnameShouldBe(string expectedSurname)
    {
        _result.Surname.Should().Be(expectedSurname);
    }

    [Then(@"The user's username should be ""(.*)""")]
    public void ThenTheUserUsernameShouldBe(string expectedUsername)
    {
        _result.UserName.Should().Be(expectedUsername);
    }

    [Then(@"The user's culture should be ""(.*)""")]
    public void ThenTheUserCultureShouldBe(string expectedCulture)
    {
        _result.Culture.Should().Be(expectedCulture);
    }

    [Then("The user's notification status should be (.*)")]
    public void ThenTheUserNotificationStatusShouldBe(bool expectedNotificationsStatus)
    {
        _result.NotificationsEnabled.Should().Be(expectedNotificationsStatus);
    }
}
