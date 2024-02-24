using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Domain.Exceptions;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserReadServiceTests
{
    private readonly IdentityUserManager _manager;

    private readonly IIdentityUserRepository _repository;

    private readonly Lazy<IIdentityUserReadService> _lazySut;

    private IIdentityUserReadService Sut => _lazySut.Value;

    public IdentityUserReadServiceTests()
    {
        _repository = Substitute.For<IIdentityUserRepository>();
        _manager = Substitute.For<IdentityUserManager>(_repository);
        _lazySut = new Lazy<IIdentityUserReadService>(() => new IdentityUserReadService(new IdentityUserMapper(), _manager, _repository));
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
    public async Task GetByIdAsync_WhenUserWithGivenIdNotExist_ThenExceptionThrown()
    {
        var action = async () =>
        {
            await Sut.GetByIdAsync(Guid.NewGuid());
        };

        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
}
