using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;
using Eclipse.Tests.Utils;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserReadServiceTests
{
    private readonly IUserRepository _repository;

    private readonly Lazy<IUserReadService> _lazySut;

    private IUserReadService Sut => _lazySut.Value;

    public UserReadServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();

        _lazySut = new Lazy<IUserReadService>(
            () => new UserReadService(
                new UserManager(_repository),
                _repository
            ));
    }

    [Fact]
    public async Task GetAllAsync_WhenUsersExists_ThenProperDataReturned()
    {
        var count = 5;
        var users = UserGenerator.Generate(count);
        _repository.GetAllAsync().Returns(Task.FromResult<IReadOnlyList<User>>(users));

        var result = await Sut.GetAllAsync();

        result.Count.Should().Be(count);
        result.All(r => users.Any(u => u.Id == r.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserWithGivenIdNotExist_ThenFailureResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(User));

        var result = await Sut.GetByIdAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();

        ErrorComparer.AreEqual(result.Error, expectedError);
    }
}
