using Eclipse.Core.Core;
using Eclipse.Core.CurrentUser;
using Eclipse.Core.Models;

using FluentAssertions;

using Xunit;

namespace Eclipse.Core.Tests;

public class CurrentTelegramUserTests
{
    private readonly ICurrentTelegramUser _sut = new CurrentTelegramUser();

    [Fact]
    public void GetCurrentUser_WhenUserSet_THenProperDataReturned()
    {
        var user = new TelegramUser(15, "test", "test", "test");
        _sut.SetCurrentUser(user);
        var result = _sut.GetCurrentUser();

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Surname.Should().Be(user.Surname);
        result.Username.Should().Be(user.Username);
    }
}
