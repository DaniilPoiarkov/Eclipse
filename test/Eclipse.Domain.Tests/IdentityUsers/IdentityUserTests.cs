using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
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
        
        result.Text.Should().Be("test");
        result.Id.Should().NotBeEmpty();
        result.UserId.Should().Be(_sut.Id);
        _sut.TodoItems.Count.Should().Be(1);
    }

    [Fact]
    public void AddTodoItem_WhenTextIsNull_ThenExceptionThrown()
    {
        var action = () =>
        {
            _sut.AddTodoItem(null);
        };

        action.Should().Throw<TodoItemValidationException>();
    }

    [Fact]
    public void AddTodoItem_WhenTextViolatesLengthRestrictions_ThenValidationExceptionThrown()
    {
        var text = new string('x', TodoItemConstants.MaxLength + 1);

        var maxLengthAction = () =>
        {
            _sut.AddTodoItem(text);
        };

        var minLengthAction = () =>
        {
            _sut.AddTodoItem(string.Empty);
        };

        maxLengthAction.Should().Throw<TodoItemValidationException>();
        minLengthAction.Should().Throw<TodoItemValidationException>();
    }

    [Fact]
    public void FinishItem_WhenItemExists_ThenItemRemovedFromCollection()
    {
        var item = _sut.AddTodoItem("test");

        var result = _sut.FinishItem(item.Id);

        _sut.TodoItems.Should().BeEmpty();
        result.Id.Should().Be(item.Id);
    }

    [Fact]
    public void FinishItem_WhenItemWithSpecifiedIdNotExists_ThenExceptionThrown()
    {
        var action = () =>
        {
            _sut.FinishItem(Guid.NewGuid());
        };

        action.Should().Throw<EntityNotFoundException>();
    }
}
