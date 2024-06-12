using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Results;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class CachedUserServiceTests
{
    private readonly IUserCache _userCache;

    private readonly IUserService _userService;

    private readonly Lazy<IUserService> _lazySut;

    private IUserService Sut => _lazySut.Value;

    public CachedUserServiceTests()
    {
        _userCache = Substitute.For<IUserCache>();
        _userService = Substitute.For<IUserService>();
        
        _lazySut = new Lazy<IUserService>(() => new CachedUserService(_userCache, _userService));
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ThenResultCached()
    {
        var dto = GetDto();

        var createDto = new UserCreateDto();

        _userService.CreateAsync(createDto)
            .Returns(
                Task.FromResult(Result<UserDto>.Success(dto)
            ));

        var result = await Sut.CreateAsync(createDto);

        result.IsSuccess.Should().BeTrue();
        await _userService.Received().CreateAsync(createDto);
        await _userCache.Received().AddOrUpdateAsync(result);
    }

    [Fact]
    public async Task GetByChatIdAsync_WhenUserCached_ThenCachedUserReturned()
    {
        var dto = GetDto();

        _userCache.GetByChatIdAsync(dto.ChatId).Returns(dto);

        var result = await Sut.GetByChatIdAsync(dto.ChatId);

        await _userCache.Received().GetByChatIdAsync(dto.ChatId);
        await _userService.DidNotReceive().GetByChatIdAsync(dto.ChatId);
        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(dto.ChatId);
    }

    [Fact]
    public async Task GetByChatIdAsync_WhenUserIsNotCached_ThenServiceCalled_AndCacheUser()
    {
        var dto = GetDto();

        _userService.GetByChatIdAsync(dto.ChatId)
            .Returns(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.GetByChatIdAsync(dto.ChatId);

        await _userService.Received().GetByChatIdAsync(dto.ChatId);
        await _userCache.Received().AddOrUpdateAsync(dto);
        await _userCache.Received().GetByChatIdAsync(dto.ChatId);

        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(dto.ChatId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserCached_ThenCachedUserReturned()
    {
        var dto = GetDto();

        _userCache.GetByIdAsync(dto.Id).Returns(dto);

        var result = await Sut.GetByIdAsync(dto.Id);

        await _userCache.Received().GetByIdAsync(dto.Id);
        await _userService.DidNotReceive().GetByIdAsync(dto.Id);
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(dto.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserIsNotCached_ThenServiceCalled_AndUserCached()
    {
        var dto = GetDto();

        _userService.GetByIdAsync(dto.Id)
            .Returns(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.GetByIdAsync(dto.Id);

        await _userCache.Received().GetByIdAsync(dto.Id);
        await _userService.Received().GetByIdAsync(dto.Id);
        await _userCache.Received().AddOrUpdateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(dto.Id);
    }

    [Fact]
    public async Task SetUserGmtTimeAsync_WhenCalled_ThenUpdatedUserCached()
    {
        var dto = GetDto();
        var time = new TimeOnly();

        _userService.SetUserGmtTimeAsync(dto.Id, time)
            .Returns(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.SetUserGmtTimeAsync(dto.Id, time);

        await _userService.Received().SetUserGmtTimeAsync(dto.Id, time);
        await _userCache.Received().AddOrUpdateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(dto.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_ThenUpdatedUserCached()
    {
        var dto = GetDto();

        var updateDto = new UserUpdateDto();

        _userService.UpdateAsync(dto.Id, updateDto)
            .Returns(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.UpdateAsync(dto.Id, updateDto);

        result.IsSuccess.Should().BeTrue();

        await _userService.Received().UpdateAsync(dto.Id, updateDto);
        await _userCache.Received().AddOrUpdateAsync(dto);
        result.Value.Id.Should().Be(dto.Id);
    }

    private static UserDto GetDto() => UserDtoGenerator.Generate(1, 1).First();
}
