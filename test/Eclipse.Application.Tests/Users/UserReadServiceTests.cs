using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Common.Clock;
using Eclipse.Common.Linq;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserReadServiceTests
{
    private readonly IUserRepository _repository;

    private readonly ITimeProvider _timeProvider;

    private readonly UserReadService _sut;

    public UserReadServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new UserReadService(_repository, _timeProvider);
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

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ThenReturns()
    {
        var user = UserGenerator.Get();

        _repository.FindAsync(user.Id).Returns(user);

        var expected = user.ToDto();

        var result = await _sut.GetByIdAsync(user.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByChatIdAsync_WhenUserExists_ThenReturns()
    {
        var user = UserGenerator.Get();

        _repository.FindByChatIdAsync(user.ChatId).Returns(user);

        var expected = user.ToDto();

        var result = await _sut.GetByChatIdAsync(user.ChatId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetByChatIdAsync_WhenUserNotExists_ThenFailedRetured(long chatId)
    {
        var result = await _sut.GetByChatIdAsync(chatId);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(DefaultErrors.EntityNotFound<User>());
    }

    [Theory]
    [InlineData(10, 5)]
    public async Task GetListAsync_WhenPaginationSpecified_ThenProperListReturned(int pageSize, int page)
    {
        var request = new PaginationRequest<GetUsersRequest>
        {
            PageSize = pageSize,
            Page = page
        };

        var users = UserGenerator.Generate(pageSize);

        _repository.GetByExpressionAsync(
            Arg.Any<Expression<Func<User, bool>>>(), (page - 1) * pageSize, pageSize
        ).Returns(users);

        _repository.CountAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(pageSize * page);

        var expected = users.Select(u => u.ToSlimDto());

        var result = await _sut.GetListAsync(request);

        result.Items.Should().BeEquivalentTo(expected);
        result.Pages.Should().Be(page);
        result.TotalCount.Should().Be(pageSize * page);
    }
}
