using Eclipse.Application.Users.Services;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserReadServiceTests
{
    private readonly IUserRepository _repository;

    private readonly UserReadService _sut;

    public UserReadServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();

        _sut = new UserReadService(_repository);
    }

    [Theory]
    [InlineData(5)]
    public async Task GetAllAsync_WhenUsersExists_ThenProperDataReturned(int count)
    {
        var users = UserGenerator.Generate(count);

        _repository.GetAllAsync().Returns(users);

        var result = await _sut.GetAllAsync();

        result.Count.Should().Be(count);
        result.All(r => users.Any(u => u.Id == r.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserWithGivenIdNotExist_ThenFailureResultReturned()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }
}
