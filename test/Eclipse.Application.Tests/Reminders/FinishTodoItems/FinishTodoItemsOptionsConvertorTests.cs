using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Tests.Generators;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.FinishTodoItems;

public sealed class FinishTodoItemsOptionsConvertorTests
{
    private readonly FinishTodoItemsOptionsConvertor _sut;

    [Fact]
    public void Convert_WhenCalled_ThenConverts()
    {
        var user = UserGenerator.Get();
        var result = _sut.Convert(user);

        result.UserId.Should().Be(user.Id);
        result.Gmt.Should().Be(user.Gmt);
    }
}
