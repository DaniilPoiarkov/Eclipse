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
    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<IIdentityUserCreateUpdateService> _lazySut;

    private IIdentityUserCreateUpdateService Sut => _lazySut.Value;

    public IdentityUserCreateUpdateServiceTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();
        
        _lazySut = new Lazy<IIdentityUserCreateUpdateService>(
            () => new IdentityUserCreateUpdateService(new IdentityUserMapper(), new IdentityUserManager(_repository))
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
        await _repository.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task UpdateAsync_WhenUsernameChanged_ThenUserUpdated()
    {
        var user = IdentityUserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id).Returns(Task.FromResult<IdentityUser?>(user));
        _repository.UpdateAsync(user).Returns(Task.FromResult(user));

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

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
    }
}
