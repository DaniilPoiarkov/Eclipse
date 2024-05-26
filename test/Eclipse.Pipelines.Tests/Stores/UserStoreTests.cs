using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Core.Models;
using Eclipse.Domain.Users;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Pipelines.Users;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Stores;

public sealed class UserStoreTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();

    private readonly IUserCache _userCache = Substitute.For<IUserCache>();

    private readonly Lazy<IUserStore> _lazySut;

    private IUserStore Sut => _lazySut.Value;

    private readonly TelegramUser User = new(1, "Name", "Surname", "Username");

    public UserStoreTests()
    {
        _lazySut = new Lazy<IUserStore>(() => new UserStore(_userService, _userCache));
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotExists_ThenCreatesUser_AndAddToCache()
    {
        _userCache.GetAllAsync()
            .Returns([]);

        _userService.GetByChatIdAsync(User.Id)
            .Returns(
                Task.FromResult(
                    Result<UserDto>.Failure(
                        DefaultErrors.EntityNotFound(typeof(User))
                    )
                ));

        var dto = GetDto();

        _userService.CreateAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdateAsync(User);

        result.IsSuccess.Should().BeTrue();

        await _userService.ReceivedWithAnyArgs()
            .CreateAsync(new UserCreateDto());

        await _userCache.Received().AddOrUpdateAsync(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserExists_AndDataSame_ThenUpdatesCache_AndNotCallService()
    {
        var dto = GetDto();

        _userCache.GetByChatIdAsync(User.Id).Returns(dto);

        var result = await Sut.AddOrUpdateAsync(new TelegramUser(1, dto.Name, dto.Surname, dto.UserName));

        result.IsSuccess.Should().BeTrue();

        await _userService.DidNotReceiveWithAnyArgs()
            .UpdateAsync(dto.Id, new UserUpdateDto());

        await _userCache.Received().AddOrUpdateAsync(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserHasNewData_ThenCallsService_AndUpdatesCache()
    {
        var dto = GetDto();

        _userCache.GetByChatIdAsync(User.Id).Returns(dto);

        var update = GetUpdateDto();

        _userService.UpdateAsync(dto.Id, update)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdateAsync(User);

        result.IsSuccess.Should().BeTrue();

        await _userService.ReceivedWithAnyArgs()
            .UpdateAsync(dto.Id, update);

        await _userCache.Received().AddOrUpdateAsync(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotCached_AndExistsInSystem__AndHasNewData_ThenCallsService_AndUpdatesCache()
    {
        _userCache.GetAllAsync()
            .Returns([]);

        var dto = GetDto();

        _userService.GetByChatIdAsync(User.Id)
            .Returns(dto);

        var update = GetUpdateDto();

        _userService.UpdateAsync(dto.Id, update)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<UserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdateAsync(User);

        result.IsSuccess.Should().BeTrue();

        await _userService.ReceivedWithAnyArgs()
            .UpdateAsync(dto.Id, update);

        await _userCache.Received().AddOrUpdateAsync(dto);
    }

    private UserDto GetDto()
    {
        return new UserDto
        {
            Id = Guid.NewGuid(),
            Name = "old_name",
            Surname = "old_surname",
            UserName = "old_username",
            ChatId = User.Id,
        };
    }

    private static UserUpdateDto GetUpdateDto()
    {
        return new UserUpdateDto
        {
            Name = "new_name",
            Surname = "new_surname",
            UserName = "new_username",
        };
    }
}
