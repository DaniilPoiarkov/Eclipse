using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.TodoItems;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.IdentityUsers;

public class IdentityUserTests
{
    private readonly IdentityUser _sut;

    public IdentityUserTests()
    {
        _sut = IdentityUserGenerator.Generate(1).First();
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
        var expectedError = Error.Validation($"IdentityUser.AddTodoItem.{errorCode}", $"TodoItem:{errorCode}");

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
}
