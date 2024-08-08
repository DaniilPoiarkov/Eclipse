using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserCreateUpdateServiceTests
{
    private readonly IUserRepository _repository;

    private readonly Lazy<IUserCreateUpdateService> _lazySut;

    private IUserCreateUpdateService Sut => _lazySut.Value;

    public UserCreateUpdateServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();

        _lazySut = new Lazy<IUserCreateUpdateService>(
            () => new UserCreateUpdateService(
                new UserManager(_repository)
            ));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var expected = DefaultErrors.EntityNotFound(typeof(User));
        var result = await Sut.UpdateAsync(Guid.NewGuid(), new UserUpdateDto());

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(expected, result.Error);

        await _repository.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNameChanged_ThenUserUpdated()
    {
        var user = UserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<User?>(user));

        _repository.UpdateAsync(user)
            .Returns(Task.FromResult(user));

        var updateDto = new UserUpdateDto
        {
            UserName = "new_username",
            Name = "new_name",
            Surname = "new_surname"
        };

        var result = await Sut.UpdateAsync(user.Id, updateDto);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;
        value.UserName.Should().Be(updateDto.UserName);
        value.Name.Should().Be(updateDto.Name);
        value.Surname.Should().Be(updateDto.Surname);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [InlineData("new_name", "new_surname", "new_user_name", "en", true)]
    [InlineData("John", "Doe", "JohnDoe", "en", true)]
    [InlineData("Billy", "Jean", "Is_Not_My_Lover", "uk", false)]
    public async Task UpdateAsync_WhenModelIsValid_ThenUpdatedSuccessfully(string name, string surname, string userName, string culture, bool notificationsEnabled)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var model = new UserUpdateDto
        {
            Name = name,
            Surname = surname,
            UserName = userName,
            Culture = culture,
            NotificationsEnabled = notificationsEnabled,
        };

        var result = await Sut.UpdateAsync(user.Id, model);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;

        value.Id.Should().Be(user.Id);
        value.Name.Should().Be(name);
        value.Surname.Should().Be(surname);
        value.UserName.Should().Be(userName);
        value.Culture.Should().Be(culture);
        value.NotificationsEnabled.Should().Be(notificationsEnabled);
        value.Gmt.Should().Be(user.Gmt);

        user.Name.Should().Be(name);
        user.Surname.Should().Be(surname);
        user.UserName.Should().Be(userName);
        user.Culture.Should().Be(culture);
        user.NotificationsEnabled.Should().Be(notificationsEnabled);

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

        var result = await Sut.UpdateAsync(user.Id, model);

        result.IsSuccess.Should().BeFalse();
        ErrorComparer.AreEqual(result.Error, expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNameIsUsed_ThenErrorReturned()
    {
        var expected = Error.Validation("Users.Update", "User:DuplicateData", "UserName");

        var user = UserGenerator.Get(1);
        var user2 = UserGenerator.Get(2);

        _repository.GetByExpressionAsync(_ => true)
            .ReturnsForAnyArgs([user2]);

        _repository.FindAsync(user.Id).Returns(user);

        var model = new UserUpdateDto
        {
            Name = user.Name,
            UserName = user2.UserName,
            Culture = user.Culture,
            Surname = user.Surname,
            NotificationsEnabled = user.NotificationsEnabled,
        };

        var result = await Sut.UpdateAsync(user.Id, model);

        result.IsSuccess.Should().BeFalse();
        ErrorComparer.AreEqual(result.Error, expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }

    [Theory]
    [InlineData(true, "Name", false, null, false, null, true, "en", false, false)]
    [InlineData(true, "Name", true, "Surname", true, "MegaBrain", true, "en", true, false)]
    [InlineData(true, "Name", true, "Surname", false, "NotSoMegaBrain", false, "en", true, true)]
    [InlineData(false, "Name", false, "Surname", false, "NotSoMegaBrain", false, "en", false, true)]
    public async Task UpdatePartialAsync_WhenModelValid_ThenUpdatedSuccessfully(
        bool nameChanged,
        string? name,
        bool surnameChanged,
        string? surname,
        bool userNameChanged,
        string? userName,
        bool cultureChanged,
        string? culture,
        bool notificationsEnabledChanged,
        bool notificationsEnabled)
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);
        _repository.UpdateAsync(user).Returns(user);

        var model = new UserPartialUpdateDto
        {
            NameChanged = nameChanged,
            Name = name,

            SurnameChanged = surnameChanged,
            Surname = surname,
            
            UserNameChanged = userNameChanged,
            UserName = userName,

            CultureChanged = cultureChanged,
            Culture = culture,

            NotificationsEnabledChanged = notificationsEnabledChanged,
            NotificationsEnabled = notificationsEnabled
        };

        var result = await Sut.UpdatePartialAsync(user.Id, model);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;

        value.Id.Should().Be(user.Id);

        value.Name.Should().Be(user.Name).And
            .Subject.Should().Be(nameChanged ? name : user.Name);

        value.Surname.Should().Be(user.Surname).And
            .Subject.Should().Be(surnameChanged ? surname : user.Surname);

        value.UserName.Should().Be(user.UserName).And
            .Subject.Should().Be(userNameChanged ? userName : user.UserName);

        value.Culture.Should().Be(user.Culture).And
            .Subject.Should().Be(cultureChanged ? culture : user.Culture);

        value.NotificationsEnabled.Should().Be(user.NotificationsEnabled).And
            .Subject.Should().Be(notificationsEnabledChanged ? notificationsEnabled : user.NotificationsEnabled);

        await _repository.Received().UpdateAsync(user);
    }

    [Theory]
    [InlineData(true, null, false, null, false, null, true, "en", false, false, "Name")]
    [InlineData(true, "Name", true, null, true, null, true, "uk", true, false, "UserName")]
    public async Task UpdatePartialAsync_WhenModelInvalid_TheErrorReturned(
        bool nameChanged,
        string? name,
        bool surnameChanged,
        string? surname,
        bool userNameChanged,
        string? userName,
        bool cultureChanged,
        string? culture,
        bool notificationsEnabledChanged,
        bool notificationsEnabled,
        object arg)
    {
        var expected = Error.Validation("Users.Update", "{0}IsRequired", arg);
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var model = new UserPartialUpdateDto
        {
            NameChanged = nameChanged,
            Name = name,

            SurnameChanged = surnameChanged,
            Surname = surname,

            UserNameChanged = userNameChanged,
            UserName = userName,

            CultureChanged = cultureChanged,
            Culture = culture,

            NotificationsEnabledChanged = notificationsEnabledChanged,
            NotificationsEnabled = notificationsEnabled
        };

        var result = await Sut.UpdatePartialAsync(user.Id, model);

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(result.Error, expected);
        await _repository.DidNotReceive().UpdateAsync(user);
    }
}
