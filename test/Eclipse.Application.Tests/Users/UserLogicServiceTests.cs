using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserLogicServiceTests
{
    private readonly IUserRepository _repository;

    private readonly Lazy<IUserLogicService> _lazySut;

    private IUserLogicService Sut => _lazySut.Value;

    public UserLogicServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _lazySut = new Lazy<IUserLogicService>(
            () => new UserLogicService(
                new UserManager(_repository)
            ));
    }

    [Theory]
    [InlineData(-4)]
    [InlineData(4)]
    [InlineData(11)]
    public async Task SetUserGmtTimeAsync_WhenTimeIsValid_ThenUpdatedSuccessfully(int time)
    {
        var user = UserGenerator.Generate(1).First();

        _repository.FindAsync(user.Id)
            .Returns(Task.FromResult<User?>(user));

        var utc = DateTime.UtcNow;
        var hour = (utc.Hour + time + 24) % 24;

        var currentUserTime = new TimeOnly(hour, utc.Minute);
        var expected = new TimeSpan(time, 0, 0);

        var result = await Sut.SetUserGmtTimeAsync(user.Id, currentUserTime);

        await _repository.Received().FindAsync(user.Id);
        await _repository.Received().UpdateAsync(user);
        result.IsSuccess.Should().BeTrue();
        result.Value.Gmt.Should().Be(expected);
    }
}
