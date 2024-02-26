using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.IdentityUsers.Services;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserReadServiceTests
{
    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<IIdentityUserReadService> _lazySut;

    private IIdentityUserReadService Sut => _lazySut.Value;

    public IdentityUserReadServiceTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();

        _lazySut = new Lazy<IIdentityUserReadService>(
            () => new IdentityUserReadService(
                new IdentityUserMapper(),
                new IdentityUserManager(_repository),
                _repository
            ));
    }

    [Fact]
    public async Task GetAllAsync_WhenUsersExists_ThenProperDataReturned()
    {
        var count = 5;
        var users = IdentityUserGenerator.Generate(count);
        _repository.GetAllAsync().Returns(Task.FromResult<IReadOnlyList<IdentityUser>>(users));

        var result = await Sut.GetAllAsync();

        result.Count.Should().Be(count);
        result.All(r => users.Any(u => u.Id == r.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserWithGivenIdNotExist_ThenFailureResultReturned()
    {
        var expectedError = DefaultErrors.EntityNotFound(typeof(IdentityUser));

        var result = await Sut.GetByIdAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        var error = result.Error;

        error.Should().NotBeNull();
        error!.Code.Should().Be(expectedError.Code);
        error.Description.Should().Be(expectedError.Description);
        error.Args.Should().BeEquivalentTo(expectedError.Args);
        error.Type.Should().Be(expectedError.Type);
    }
}
