using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.IdentityUsers;

using NSubstitute;

namespace Eclipse.Tests.Bdd.Application.IdentityUsers.StepDefinitions;

[Binding]
internal sealed class UpdateAsyncStepDefinitions
{
    private readonly Lazy<IdentityUserManager> _lazyManager;

    private readonly IIdentityUserRepository _repository;

    private readonly IdentityUserUpdateDto _updateDto = new();

    private readonly Lazy<IIdentityUserService> _lazySut;

    private IIdentityUserService _sut => _lazySut.Value;

    private IdentityUserDto _result = new();

    private IdentityUser _user = default!;

    public UpdateAsyncStepDefinitions()
    {
        _repository = Substitute.For<IIdentityUserRepository>();

        _lazyManager = new Lazy<IdentityUserManager>(() => new IdentityUserManager(_repository));

        _lazySut = new Lazy<IIdentityUserService>(() => new IdentityUserService(new IdentityUserMapper(), _lazyManager.Value));
    }

    [Given("An existing user with followind data:")]
    public void GivenAUserWithId(Table table)
    {
        _user = IdentityUser.Create(
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
        _updateDto.Username = table.Rows[0]["Username"];
        _updateDto.Culture = table.Rows[0]["Culture"];
        _updateDto.NotificationsEnabled = bool.Parse(table.Rows[0]["NotificationsEnabled"]);

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
        _result.Username.Should().Be(expectedUsername);
    }

    [Then(@"The user's culture should be ""(.*)""")]
    public void ThenTheUserCultureShouldBe(string expectedCulture)
    {
        _result.Culture.Should().Be(expectedCulture);
    }

    [Then("The user's notification status should be (.*)")]
    public void ThenTheUserSurnameShouldBe(bool expectedNotificationsStatus)
    {
        _result.NotificationsEnabled.Should().Be(expectedNotificationsStatus);
    }
}
