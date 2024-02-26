using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class CachedIdentityUserServiceTests
{
    private readonly IIdentityUserCache _userCache;

    private readonly IIdentityUserService _userService;

    private readonly Lazy<IIdentityUserService> _lazySut;

    private IIdentityUserService Sut => _lazySut.Value;

    public CachedIdentityUserServiceTests()
    {
        _userCache = Substitute.For<IIdentityUserCache>();
        _userService = Substitute.For<IIdentityUserService>();

        _lazySut = new Lazy<IIdentityUserService>(() => new CachedIdentityUserService(_userCache, _userService));
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ThenResultCached()
    {
        var dto = GetDto();

        var createDto = new IdentityUserCreateDto();

        _userService.CreateAsync(createDto)
            .Returns(
                Task.FromResult(Result<IdentityUserDto>.Success(dto)
            ));

        var result = await Sut.CreateAsync(createDto);

        result.IsSuccess.Should().BeTrue();
        await _userService.Received().CreateAsync(createDto);
        _userCache.Received().AddOrUpdate(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ThenAllUsersCached()
    {
        var dtos = IdentityUserDtoGenerator.GenerateUsers(1, 5);

        _userService.GetAllAsync()
            .Returns(
                Task.FromResult<IReadOnlyList<IdentityUserDto>>(dtos)
            );

        var result = await Sut.GetAllAsync();

        foreach (var dto in result)
        {
            _userCache.Received().AddOrUpdate(dto);
        }

        await _userService.Received().GetAllAsync();
    }

    [Fact]
    public async Task GetByChatIdAsync_WhenUserCached_ThenCachedUserReturned()
    {
        var dto = GetDto();

        _userCache.GetByChatId(dto.ChatId).Returns(dto);

        var result = await Sut.GetByChatIdAsync(dto.ChatId);

        _userCache.Received().GetByChatId(dto.ChatId);
        await _userService.DidNotReceive().GetByChatIdAsync(dto.ChatId);
        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(dto.ChatId);
    }

    [Fact]
    public async Task GetByChatId_WhenUserIsNotCached_ThenServiceCalled_AndCacheUser()
    {
        var dto = GetDto();

        _userService.GetByChatIdAsync(dto.ChatId)
            .Returns(
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.GetByChatIdAsync(dto.ChatId);

        await _userService.Received().GetByChatIdAsync(dto.ChatId);
        _userCache.Received().AddOrUpdate(dto);
        _userCache.Received().GetByChatId(dto.ChatId);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(dto.ChatId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserCached_ThenCachedUserReturned()
    {
        var dto = GetDto();

        _userCache.GetById(dto.Id).Returns(dto);

        var result = await Sut.GetByIdAsync(dto.Id);

        _userCache.Received().GetById(dto.Id);
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
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.GetByIdAsync(dto.Id);

        _userCache.Received().GetById(dto.Id);
        await _userService.Received().GetByIdAsync(dto.Id);
        _userCache.Received().AddOrUpdate(dto);

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
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.SetUserGmtTimeAsync(dto.Id, time);

        await _userService.Received().SetUserGmtTimeAsync(dto.Id, time);
        _userCache.Received().AddOrUpdate(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(dto.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_ThenUpdatedUserCached()
    {
        var dto = GetDto();

        var updateDto = new IdentityUserUpdateDto();

        _userService.UpdateAsync(dto.Id, updateDto)
            .Returns(
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.UpdateAsync(dto.Id, updateDto);

        result.IsSuccess.Should().BeTrue();

        await _userService.Received().UpdateAsync(dto.Id, updateDto);
        _userCache.Received().AddOrUpdate(dto);
        result.Value.Id.Should().Be(dto.Id);
    }

    private static IdentityUserDto GetDto() => IdentityUserDtoGenerator.GenerateUsers(1, 1).First();
}
