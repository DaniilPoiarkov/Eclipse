using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserCreateUpdateServiceTests
{
    private readonly IdentityUserManager _manager;

    private readonly Lazy<IIdentityUserCreateUpdateService> _lazySut;

    private IIdentityUserCreateUpdateService Sut => _lazySut.Value;

    public IdentityUserCreateUpdateServiceTests()
    {
        _manager = Substitute.For<IdentityUserManager>(
            Substitute.For<IIdentityUserRepository>());

        _lazySut = new Lazy<IIdentityUserCreateUpdateService>(
            () => new IdentityUserCreateUpdateService(new IdentityUserMapper(), _manager)
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var action = async () =>
        {
            await Sut.UpdateAsync(Guid.NewGuid(), new IdentityUserUpdateDto());
        };

        await action.Should().ThrowAsync<EntityNotFoundException>();
        await _manager.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task UpdateAsync_WhenUsernameChanged_ThenUserUpdated()
    {
        var user = IdentityUserGenerator.Generate(1).First();

        _manager.FindByIdAsync(user.Id).Returns(Task.FromResult<IdentityUser?>(user));
        _manager.UpdateAsync(user).Returns(Task.FromResult<IdentityUser?>(user));

        var updateDto = new IdentityUserUpdateDto
        {
            Username = "new_username",
            Name = "new_name",
            Surname = "new_surname"
        };

        var result = await Sut.UpdateAsync(user.Id, updateDto);

        result.Username.Should().Be(updateDto.Username);
        result.Name.Should().Be(updateDto.Name);
        result.Surname.Should().Be(updateDto.Surname);

        await _manager.Received().FindByIdAsync(user.Id);
        await _manager.Received().UpdateAsync(user);
    }
}
