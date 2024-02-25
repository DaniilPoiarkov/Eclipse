using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserLogicServiceTests
{
    private readonly IdentityUserManager _manager;

    private readonly Lazy<IIdentityUserLogicService> _lazySut;

    private IIdentityUserLogicService Sut => _lazySut.Value;

    public IdentityUserLogicServiceTests()
    {
        _manager = Substitute.For<IdentityUserManager>(
            Substitute.For<IIdentityUserRepository>());

        _lazySut = new Lazy<IIdentityUserLogicService>(() => new IdentityUserLogicService(new IdentityUserMapper(), _manager));
    }

    [Fact]
    public async Task SetUserGmtTimeAsync_WhenTimeIsValid_ThenUpdatedSuccessfully()
    {
        var user = IdentityUserGenerator.Generate(1).First();

        _manager.FindByIdAsync(user.Id).Returns(Task.FromResult<IdentityUser?>(user));

        var utc = DateTime.UtcNow;

        var hour = utc.Hour - 4 < 0
            ? utc.Hour + 20
            : utc.Hour - 4;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(-4, 0, 0);

        var result = await Sut.SetUserGmtTimeAsync(user.Id, currentUserTime);

        await _manager.Received().FindByIdAsync(user.Id);
        await _manager.Received().UpdateAsync(user);
        result.Gmt.Should().Be(expected);
    }
}
