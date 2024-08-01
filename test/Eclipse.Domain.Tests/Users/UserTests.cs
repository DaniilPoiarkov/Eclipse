using Eclipse.Common.Results;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.Users;

public class UserTests
{
    private readonly User _sut;

    public UserTests()
    {
        _sut = UserGenerator.Generate(1).First();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-3)]
    [InlineData(-4)]
    [InlineData(-5)]
    [InlineData(-6)]
    [InlineData(-7)]
    [InlineData(-8)]
    [InlineData(-9)]
    [InlineData(-10)]
    [InlineData(-11)]
    public void SetGmt_WhenLocalTimeSpecified_ThenProperCalculationAccordingToUtcSet(int gmt)
    {
        var utc = DateTime.UtcNow;

        var hour = (utc.Hour + gmt + 24) % 24;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = TimeSpan.FromHours(gmt);

        _sut.SetGmt(currentUserTime);

        _sut.Gmt.Should().Be(expected);
    }

    [Fact]
    public void AddReminder_WhenReminderCreated_ThenValidDataReturned_AndAddedToCollection()
    {
        var result = _sut.AddReminder("test", TimeOnly.FromDateTime(DateTime.UtcNow));

        result.Id.Should().NotBeEmpty();
        result.Text.Should().Be("test");
        result.UserId.Should().Be(_sut.Id);
    }

    [Fact]
    public void RemoveRemindersForTime_WhenUserHasReminder_ThenReminderRemoved()
    {
        var time = new TimeOnly(5, 0, 0);

        _sut.AddReminder("test", time);
        _sut.AddReminder("test", time.AddMinutes(1));
        _sut.AddReminder("test", time.AddMinutes(1));

        var result = _sut.RemoveRemindersForTime(time);

        _sut.Reminders.Count.Should().Be(2);
        _sut.Reminders.Any(r => r.NotifyAt == time).Should().BeFalse();
        result.Count.Should().Be(1);
        result[0].NotifyAt.Should().Be(time);
    }

    [Theory]
    [InlineData("t")]
    [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest")]
    [InlineData("Some regular text! With __dif3r3nt &^% characters!)(_++_*@")]
    public void AddTodoItem_WhenTextValid_ThenTodoItemCreated(string text)
    {
        var result = _sut.AddTodoItem(text);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;
        value.Text.Should().Be(text);
        value.Id.Should().NotBeEmpty();
        value.UserId.Should().Be(_sut.Id);
        _sut.TodoItems.Count.Should().Be(1);
    }

    [Theory]
    [InlineData(null, "Empty")]
    [InlineData("", "Empty")]
    [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest", "MaxLength")]
    public void AddTodoItem_WhenDataInvalid_ThenFailureResultReturned(string? text, string errorCode)
    {
        var expectedError = Error.Validation($"User.AddTodoItem.{errorCode}", $"TodoItem:{errorCode}");

        var result = _sut.AddTodoItem(text);

        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public void FinishItem_WhenItemExists_ThenItemRemovedFromCollection()
    {
        var item = _sut.AddTodoItem("test");

        var result = _sut.FinishItem(item.Value.Id);

        result.IsSuccess.Should().BeTrue();

        _sut.TodoItems.Should().BeEmpty();
        result.Value.Id.Should().Be(item.Value.Id);
    }

    [Fact]
    public void FinishItem_WhenItemWithSpecifiedIdNotExists_ThenFailureResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(TodoItem));

        var result = _sut.FinishItem(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be(expectedError.Code);
        result.Error.Description.Should().Be(expectedError.Description);
        result.Error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Fact]
    public void SetSignInCode_WhenCalled_ThenNewCodeSet()
    {
        var setTime = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 0));
        var expectedExpirationTime = setTime.Add(UserConsts.SignInCodeExpiration);

        _sut.SetSignInCode(setTime);
        _sut.SignInCode.Should().NotBeNull();
        _sut.SignInCodeExpiresAt.Should().Be(expectedExpirationTime);
    }

    [Fact]
    public void SetSignInCode_WhenCalledMultipleTImes_ThenNotSetNewCodeUntilPreviousExpires()
    {
        var setTime = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 0));

        _sut.SetSignInCode(setTime);

        var signInCode = _sut.SignInCode;
        var expiresAt = _sut.SignInCodeExpiresAt;

        _sut.SetSignInCode(setTime.AddMinutes(1));
        _sut.SetSignInCode(setTime.AddMinutes(2));
        _sut.SetSignInCode(setTime.AddMinutes(3));

        _sut.SignInCode.Should().Be(signInCode);
        _sut.SignInCodeExpiresAt.Should().Be(expiresAt);
    }

    [Fact]
    public void SetSignInCode_WhenPreviousCodeIsExpired_ThenSetsNewCode()
    {
        var firstDate = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 0));
        _sut.SetSignInCode(firstDate);

        var signInCode = _sut.SignInCode;

        var secondDate = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 10));

        _sut.SetSignInCode(secondDate);

        _sut.SignInCode.Should().NotBe(signInCode);
        _sut.SignInCodeExpiresAt.Should().Be(secondDate.Add(UserConsts.SignInCodeExpiration));
    }

    [Fact]
    public void IsValidSignInCode_WhenCodeIsValid_ThenReturnsTrue()
    {
        _sut.SetSignInCode(DateTime.UtcNow);
        _sut.IsValidSignInCode(DateTime.UtcNow, _sut.SignInCode).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("qwerty")]
    [InlineData("1234567")]
    public void IsValidSignInCode_WhenValuesAreInvalid_ThenReturnsFalse(string? code)
    {
        _sut.SetSignInCode(DateTime.UtcNow);
        _sut.IsValidSignInCode(DateTime.UtcNow, code!).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("qwerty")]
    [InlineData("1234567")]
    public void IsValidSignInCode_WhenCodeWasNotSet_THenReturnsFalse(string? code)
    {
        _sut.IsValidSignInCode(DateTime.UtcNow, code!).Should().BeFalse();
    }

    [Fact]
    public void IsValidSignInCode_WhenCodeIsExpired_ThenReturnsFalse()
    {
        var creationDate = new DateTime(new DateOnly(1990, 1, 1), new TimeOnly(12, 0));
        var submissionDate = creationDate.Add(UserConsts.SignInCodeExpiration).AddSeconds(1);

        _sut.SetSignInCode(creationDate);

        _sut.IsValidSignInCode(submissionDate, _sut.SignInCode).Should().BeFalse();
    }
}
