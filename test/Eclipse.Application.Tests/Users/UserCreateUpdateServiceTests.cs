using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserCreateUpdateServiceTests
{
    private readonly IUserRepository _repository;

    private readonly Lazy<IUserCreateUpdateService> _lazySut;

    private IUserCreateUpdateService Sut => _lazySut.Value;

    public UserCreateUpdateServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();

        _lazySut = new Lazy<IUserCreateUpdateService>(
            () => new UserCreateUpdateService(
                new UserManager(_repository)
            ));
    }

    [Fact]
    public async Task UpdateAsync_WhenUserWithSpecifiedIdNotExist_ThenExceptionThrown()
    {
        var expected = DefaultErrors.EntityNotFound(typeof(User));
        var result = await Sut.UpdateAsync(Guid.NewGuid(), new UserUpdateDto());

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
        var user = UserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<User?>(user));

        _repository.UpdateAsync(user)
            .Returns(Task.FromResult(user));

        var updateDto = new UserUpdateDto
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
