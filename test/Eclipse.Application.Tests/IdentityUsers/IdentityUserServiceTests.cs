using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;
using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public class IdentityUserServiceTests
{
    private readonly IdentityUserManager _manager;

    private readonly Lazy<IIdentityUserService> _lazySut;

    private IIdentityUserService Sut => _lazySut.Value;

    public IdentityUserServiceTests()
    {
        _manager = Substitute.For<IdentityUserManager>(
            Substitute.For<IIdentityUserRepository>());

        _lazySut = new Lazy<IIdentityUserService>(() => new IdentityUserService(new IdentityUserMapper(), _manager));
    }

    [Fact]
    public async Task GetAllAsync_WhenUsersExists_ThenProperDataReturned()
    {
        var count = 5;
        var users = IdentityUserGenerator.Generate(count);
        _manager.GetAllAsync().Returns(Task.FromResult<IReadOnlyList<IdentityUser>>(users));

        var result = await Sut.GetAllAsync();

        result.Count.Should().Be(count);
        result.All(r => users.Any(u => u.Id == r.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var action = async () =>
        {
            await Sut.UpdateAsync(Guid.NewGuid(), new IdentityUserUpdateDto());
        };

        await action.Should().ThrowAsync<ObjectNotFoundException>();
        await _manager.DidNotReceive().UpdateAsync(default!);
    }
}
