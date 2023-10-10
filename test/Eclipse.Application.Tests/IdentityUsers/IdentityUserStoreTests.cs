using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Exceptions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Core.Models;
using Eclipse.Domain.IdentityUsers;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public class IdentityUserStoreTests
{
    private readonly IIdentityUserService _identityUserService = Substitute.For<IIdentityUserService>();

    private readonly IIdentityUserCache _identityUserCache = Substitute.For<IIdentityUserCache>();

    private readonly Lazy<IIdentityUserStore> _lazySut;

    private IIdentityUserStore Sut => _lazySut.Value;


    private readonly TelegramUser User = new(1, "Name", "Surname", "Username");

    private readonly IdentityUserDto UserDtoToUpdate = new()
    {
        Id = Guid.NewGuid(),
        Name = "N",
        Surname = "S",
        ChatId = 1,
    };


    public IdentityUserStoreTests()
    {
        _lazySut = new Lazy<IIdentityUserStore>(() => new IdentityUserStore(_identityUserService, _identityUserCache));
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotExists_ThenCreatesUser_AndAddToCache()
    {
        _identityUserCache.GetAll().Returns(new List<IdentityUserDto>());
        _identityUserService.GetByChatIdAsync(User.Id).Throws(new ObjectNotFoundException(nameof(IdentityUser)));

        var create = new IdentityUserCreateDto
        {
            ChatId = User.Id,
            Name = User.Name,
            Surname = User.Surname,
            Username = User.Username ?? string.Empty
        };

        await Sut.AddOrUpdate(User);

        var dto = await _identityUserService.ReceivedWithAnyArgs().CreateAsync(create);
        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserExists_AndDataSame_ThenUpdatesCache_AndNotCallService()
    {
        var dto = new IdentityUserDto
        {
            Id = Guid.NewGuid(),
            Name = User.Name,
            Surname = User.Surname,
            Username = User.Username!,
            ChatId = User.Id,
        };

        _identityUserCache.GetByChatId(User.Id).Returns(dto);

        await Sut.AddOrUpdate(User);

        var update = new IdentityUserUpdateDto
        {
            Name = User.Name,
            Surname = User.Surname,
            Username = User.Username,
        };

        await _identityUserService.DidNotReceive().UpdateAsync(dto.Id, update);
        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserHasNewData_ThenCallsService_AndUpdatesCache()
    {
        _identityUserCache.GetByChatId(User.Id).Returns(UserDtoToUpdate);

        await Sut.AddOrUpdate(User);

        var update = new IdentityUserUpdateDto
        {
            Name = User.Name,
            Surname = User.Surname,
            Username = User.Username,
        };

        var dto = await _identityUserService.ReceivedWithAnyArgs().UpdateAsync(UserDtoToUpdate.Id, update);
        _identityUserCache.Received().AddOrUpdate(dto);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotCached_AndExistsInSystem__AndHasNewData_ThenCallsService_AndUpdatesCache()
    {
        _identityUserCache.GetAll().Returns(new List<IdentityUserDto>());

        _identityUserService.GetByChatIdAsync(User.Id).Returns(UserDtoToUpdate);

        await Sut.AddOrUpdate(User);

        var update = new IdentityUserUpdateDto
        {
            Name = User.Name,
            Surname = User.Surname,
            Username = User.Username,
        };

        var dto = await _identityUserService.ReceivedWithAnyArgs().UpdateAsync(UserDtoToUpdate.Id, update);
        _identityUserCache.Received().AddOrUpdate(dto);
    }
}
