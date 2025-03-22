using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Tests.Users.TestData;
using Eclipse.Application.Users;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserCreateUpdateServiceTests
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly UserCreateUpdateService _sut;

    public UserCreateUpdateServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new UserCreateUpdateService(new UserManager(_repository, _timeProvider), _repository);
    }

    [Fact]
    public async Task CreateAsync_WhenFailedToInsertNewUser_ThenErrorReturned()
    {
        var expected = Error.Validation("Users.Create", "{0}IsRequired", nameof(User.Name));

        var result = await _sut.CreateAsync(new UserCreateDto());

        result.Error.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var expected = DefaultErrors.EntityNotFound<User>();
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UserUpdateDto
        {
            Name = "John",
            UserName = "john.doe"
        });

        result.Error.Should().BeEquivalentTo(expected);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNameChanged_ThenUserUpdated()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var update = new UserUpdateDto
        {
            UserName = "new_username",
            Name = "new_name",
            Surname = "new_surname"
        };

        var result = await _sut.UpdateAsync(user.Id, update);

        var value = result.Value;
        value.UserName.Should().Be(update.UserName);
        value.Name.Should().Be(update.Name);
        value.Surname.Should().Be(update.Surname);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [ClassData(typeof(UserUpdateValidTestData))]
    public async Task UpdateAsync_WhenModelIsValid_ThenUpdatedSuccessfully(UserUpdateDto model)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var result = await _sut.UpdateAsync(user.Id, model);

        user.ToDto().Should().BeEquivalentTo(result.Value);

        user.Name.Should().Be(model.Name);
        user.Surname.Should().Be(model.Surname);
        user.UserName.Should().Be(model.UserName);
        user.Culture.Should().Be(model.Culture);
        user.NotificationsEnabled.Should().Be(model.NotificationsEnabled);

        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [InlineData("", "", "Name")]
    [InlineData("", "UserName", "Name")]
    [InlineData("Name", "", "UserName")]
    public async Task UpdateAsync_WhenModelIsInvalid_ThenErrorResultReturned(string name, string userName, string arg)
    {
        var expected = Error.Validation("Users.Update", "{0}IsRequired", arg);
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var model = new UserUpdateDto
        {
            Name = name,
            UserName = userName,
            Surname = user.Surname,
            Culture = user.Culture,
            NotificationsEnabled = user.NotificationsEnabled,
        };

        var result = await _sut.UpdateAsync(user.Id, model);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNameIsUsed_ThenErrorReturned()
    {
        var expected = Error.Validation("Users.Update", "User:DuplicateData", "UserName");

        var user = UserGenerator.Get(1);
        var user2 = UserGenerator.Get(2);

        _repository.FindByUserNameAsync(user2.UserName).Returns(user2);
        _repository.FindAsync(user.Id).Returns(user);

        var model = new UserUpdateDto
        {
            Name = user.Name,
            UserName = user2.UserName,
            Culture = user.Culture,
            Surname = user.Surname,
            NotificationsEnabled = user.NotificationsEnabled,
        };

        var result = await _sut.UpdateAsync(user.Id, model);

        result.Error.Should().BeEquivalentTo(expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }

    [Theory]
    [ClassData(typeof(UserPartialUpdateValidTestData))]
    public async Task UpdatePartialAsync_WhenModelValid_ThenUpdatedSuccessfully(UserPartialUpdateDto model)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var result = await _sut.UpdatePartialAsync(user.Id, model);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;

        value.Id.Should().Be(user.Id);
        value.Name.Should().Be(user.Name);
        value.Surname.Should().Be(user.Surname);
        value.UserName.Should().Be(user.UserName);
        value.Culture.Should().Be(user.Culture);
        value.NotificationsEnabled.Should().Be(user.NotificationsEnabled);

        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [ClassData(typeof(UserPartialUpdateInvalidTestData))]
    public async Task UpdatePartialAsync_WhenInvalid_ThenErrorReturned(UserPartialUpdateDto model, object arg)
    {
        var expected = Error.Validation("Users.Update", "{0}IsRequired", arg);
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var result = await _sut.UpdatePartialAsync(user.Id, model);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }

    [Theory]
    [ClassData(typeof(UserPartialUpdateWithoutChangedFlagTestData))]
    public async Task UpdatePartialAsync_WhenDataSpecifiedButChangedSetFalse_ThenPropertyNotUpdated(UserPartialUpdateDto model)
    {
        var user = UserGenerator.Get();

        var name = user.Name;
        var surname = user.Surname;
        var userName = user.UserName;
        var culture = user.Culture;
        var notifications = user.NotificationsEnabled;

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var result = await _sut.UpdatePartialAsync(user.Id, model);

        result.IsSuccess.Should().BeTrue();

        user.Name.Should().Be(name);
        user.Surname.Should().Be(surname);
        user.UserName.Should().Be(userName);
        user.Culture.Should().Be(culture);
        user.NotificationsEnabled.Should().Be(notifications);
    }

    [Fact]
    public async Task CreateAsync_WhenUserHasNoUserName_ThenCreatedSuccessfully()
    {
        var model = new UserCreateDto
        {
            ChatId = 1,
            Name = "John",
            Surname = "Doe",
            UserName = string.Empty
        };

        var user = User.Create(Guid.NewGuid(), model.Name, model.Surname, model.UserName, model.ChatId, DateTime.UtcNow, true, true);

        _repository.CreateAsync(user).ReturnsForAnyArgs(user);

        var result = await _sut.CreateAsync(model);

        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(model.ChatId);
        result.Value.Name.Should().Be(model.Name);
        result.Value.Surname.Should().Be(model.Surname);
        result.Value.UserName.Should().Be(string.Empty);
    }

    [Theory]
    [InlineData(-4)]
    [InlineData(4)]
    [InlineData(11)]
    public async Task UpdatePartialAsync_WhenTimeIsValid_ThenUpdatedSuccessfully(int time)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var utc = DateTime.UtcNow;
        var hour = (utc.Hour + time + 24) % 24;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(time, 0, 0);

        var model = new UserPartialUpdateDto
        {
            Gmt = currentUserTime,
            GmtChanged = true,
        };

        var result = await _sut.UpdatePartialAsync(user.Id, model);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
        result.IsSuccess.Should().BeTrue();
        result.Value.Gmt.Should().Be(expected);
    }
}
