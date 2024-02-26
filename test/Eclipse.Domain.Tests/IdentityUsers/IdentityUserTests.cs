using Eclipse.Common.Results;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Shared.TodoItems;
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

    [Fact]
    public void SetGmt_WhenGmtPlus3_ThenGmtPlus3Set()
    {
        var utc = DateTime.UtcNow;

        var hour = utc.Hour + 3 > 23
            ? utc.Hour - 21
            : utc.Hour + 3;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(3, 0, 0);

        _sut.SetGmt(currentUserTime);

        _sut.Gmt.Should().Be(expected);
    }

    [Fact]
    public void SetGmt_WhenGmtMinus4_ThenGmtMinus4Set()
    {
        var utc = DateTime.UtcNow;

        var hour = utc.Hour - 4 < 0
            ? utc.Hour + 20
            : utc.Hour - 4;

        var currentUserTime = new TimeOnly(hour, utc.Minute);

        var expected = new TimeSpan(-4, 0, 0);

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

    [Fact]
    public void AddTodoItem_WhenTextValid_ThenTodoItemCreated()
    {
        var result = _sut.AddTodoItem("test");

        result.IsSuccess.Should().BeTrue();
        
        var value = result.Value;
        value.Text.Should().Be("test");
        value.Id.Should().NotBeEmpty();
        value.UserId.Should().Be(_sut.Id);
        _sut.TodoItems.Count.Should().Be(1);
    }

    [Fact]
    public void AddTodoItem_WhenTextIsNull_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("IdentityUser.AddTodoItem.Empty", "TodoItem:Empty");

        var result = _sut.AddTodoItem(null);

        result.IsSuccess.Should().BeFalse();
        
        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public void AddTodoItem_WhenTextIsTooLong_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("IdentityUser.AddTodoItem.MaxLength", "TodoItem:MaxLength", TodoItemConstants.MaxLength);
        var text = new string('x', TodoItemConstants.MaxLength + 1);

        var result = _sut.AddTodoItem(text);

        var error = result.Error;
        result.IsSuccess.Should().BeFalse();
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
    }

    [Fact]
    public void AddTodoItem_WhenTextIsEmpty_ThenFailureResultReturned()
    {
        var expectedError = Error.Validation("IdentityUser.AddTodoItem.Empty", "TodoItem:Empty");
        var result = _sut.AddTodoItem(string.Empty);

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error!.Description.Should().Be(expectedError.Description);
    }

    [Fact]
    public void FinishItem_WhenItemExists_ThenItemRemovedFromCollection()
    {
        var item = _sut.AddTodoItem("test");

        var result = _sut.FinishItem(item.Value.Id);

        result.IsSuccess.Should().BeTrue();

        _sut.TodoItems.Should().BeEmpty();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(item.Value.Id);
        result.Error.Should().BeNull();
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
