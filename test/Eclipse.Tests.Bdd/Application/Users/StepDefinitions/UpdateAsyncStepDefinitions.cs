using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using NSubstitute;

namespace Eclipse.Tests.Bdd.Application.Users.StepDefinitions;

[Binding]
internal sealed class UpdateAsyncStepDefinitions
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly UserUpdateDto _updateDto = new();

    private readonly UserCreateUpdateService _sut;

    private UserDto _result = new();

    private User _user = default!;

    public UpdateAsyncStepDefinitions()
    {
        _repository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        var manager = new UserManager(_repository, _timeProvider);

        _sut = new UserCreateUpdateService(manager, _repository);
    }

    [Given("An existing user with following data:")]
    public void GivenAUserWithId(Table table)
    {
        var row = table.Rows[0];

        _user = User.Create(
            Guid.Parse(row["Id"]),
            row["Name"],
            row["Surname"],
            row["Username"],
            long.Parse(row["ChatId"]),
            DateTime.UtcNow,
            true,
            bool.Parse(row["NewRegistered"]));

        _repository.FindAsync(_user.Id).ReturnsForAnyArgs(Task.FromResult<User?>(_user));
        _repository.UpdateAsync(_user).ReturnsForAnyArgs(Task.FromResult(_user));
    }

    [When("I update the user with the following details:")]
    public async Task WhenIUpdateTheUserWithTheFollowingDetails(Table table)
    {
        var row = table.Rows[0];

        _updateDto.Name = row["Name"];
        _updateDto.Surname = row["Surname"];
        _updateDto.UserName = row["Username"];
        _updateDto.Culture = row["Culture"];
        _updateDto.NotificationsEnabled = bool.Parse(row["NotificationsEnabled"]);

        _result = await _sut.UpdateAsync(_user.Id, _updateDto);
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
