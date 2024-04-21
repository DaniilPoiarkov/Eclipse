using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers.Services;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
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
            () => new IdentityUserCreateUpdateService(
                new IdentityUserManager(_repository)
            ));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var expected = DefaultErrors.EntityNotFound(typeof(IdentityUser));
        var result = await Sut.UpdateAsync(Guid.NewGuid(), new IdentityUserUpdateDto());

        result.IsSuccess.Should().BeFalse();

        var error = result.Error;
        error.Code.Should().Be(expected.Code);
        error.Description.Should().Be(expected.Description);
        error.Args.Should().BeEquivalentTo(expected.Args);

        await _repository.DidNotReceive().UpdateAsync(default!);
    }

    [Fact]
    public async Task UpdateAsync_WhenUsernameChanged_ThenUserUpdated()
    {
        var user = IdentityUserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<IdentityUser?>(user));

        _repository.UpdateAsync(user)
            .Returns(Task.FromResult(user));

        var updateDto = new IdentityUserUpdateDto
        {
            UserName = "new_username",
            Name = "new_name",
            Surname = "new_surname"
        };

        var result = await Sut.UpdateAsync(user.Id, updateDto);

        result.IsSuccess.Should().BeTrue();

        var value = result.Value;
        value.UserName.Should().Be(updateDto.UserName);
        value.Name.Should().Be(updateDto.Name);
        value.Surname.Should().Be(updateDto.Surname);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
    }
}
