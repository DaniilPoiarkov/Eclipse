﻿using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;
using Eclipse.Core.Models;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Shared.Errors;
using Eclipse.Pipelines.Users;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Pipelines.Tests.Stores;

public sealed class UserStoreTests
{
    private readonly IIdentityUserService _identityUserService = Substitute.For<IIdentityUserService>();

    private readonly IIdentityUserCache _identityUserCache = Substitute.For<IIdentityUserCache>();

    private readonly Lazy<IUserStore> _lazySut;

    private IUserStore Sut => _lazySut.Value;

    private readonly TelegramUser User = new(1, "Name", "Surname", "Username");

    public UserStoreTests()
    {
        _lazySut = new Lazy<IUserStore>(() => new UserStore(_identityUserService, _identityUserCache));
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotExists_ThenCreatesUser_AndAddToCache()
    {
        _identityUserCache.GetAll()
            .Returns(new List<IdentityUserDto>());

        _identityUserService.GetByChatIdAsync(User.Id)
            .Returns(
                Task.FromResult(
                    Result<IdentityUserDto>.Failure(
                        DefaultErrors.EntityNotFound(typeof(IdentityUser))
                    )
                ));

        var dto = GetDto();

        _identityUserService.CreateAsync(default!)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdate(User);

        result.IsSuccess.Should().BeTrue();

        await _identityUserService.ReceivedWithAnyArgs()
            .CreateAsync(new IdentityUserCreateDto());

        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserExists_AndDataSame_ThenUpdatesCache_AndNotCallService()
    {
        var dto = GetDto();

        _identityUserCache.GetByChatId(User.Id).Returns(dto);

        var result = await Sut.AddOrUpdate(new TelegramUser(1, dto.Name, dto.Surname, dto.Username));

        result.IsSuccess.Should().BeTrue();

        await _identityUserService.DidNotReceiveWithAnyArgs()
            .UpdateAsync(dto.Id, new IdentityUserUpdateDto());

        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserHasNewData_ThenCallsService_AndUpdatesCache()
    {
        var dto = GetDto();

        _identityUserCache.GetByChatId(User.Id).Returns(dto);

        var update = GetUpdateDto();

        _identityUserService.UpdateAsync(dto.Id, update)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdate(User);

        result.IsSuccess.Should().BeTrue();
        
        await _identityUserService.ReceivedWithAnyArgs()
            .UpdateAsync(dto.Id, update);

        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotCached_AndExistsInSystem__AndHasNewData_ThenCallsService_AndUpdatesCache()
    {
        _identityUserCache.GetAll()
            .Returns(new List<IdentityUserDto>());

        var dto = GetDto();

        _identityUserService.GetByChatIdAsync(User.Id)
            .Returns(dto);

        var update = GetUpdateDto();

        _identityUserService.UpdateAsync(dto.Id, update)
            .ReturnsForAnyArgs(
                Task.FromResult(Result<IdentityUserDto>.Success(dto))
            );

        var result = await Sut.AddOrUpdate(User);

        result.IsSuccess.Should().BeTrue();

        await _identityUserService.ReceivedWithAnyArgs()
            .UpdateAsync(dto.Id, update);

        _identityUserCache.Received().AddOrUpdate(dto);
    }

    private IdentityUserDto GetDto()
    {
        return new IdentityUserDto
        {
            Id = Guid.NewGuid(),
            Name = "old_name",
            Surname = "old_surname",
            Username = "old_username",
            ChatId = User.Id,
        };
    }

    private static IdentityUserUpdateDto GetUpdateDto()
    {
        return new IdentityUserUpdateDto
        {
            Name = "new_name",
            Surname = "new_surname",
            Username = "new_username",
        };
    }
}
