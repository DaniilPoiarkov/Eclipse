using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Linq;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserServiceTests
{
    private readonly IUserCreateUpdateService _createUpdateService;

    private readonly IUserReadService _readService;

    private readonly UserService _sut;

    public UserServiceTests()
    {
        _createUpdateService = Substitute.For<IUserCreateUpdateService>();
        _readService = Substitute.For<IUserReadService>();
        _sut = new UserService(_createUpdateService, _readService);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_ThenDelegatesToProperService()
    {
        var dto = new UserCreateDto();
        await _sut.CreateAsync(dto);
        await _createUpdateService.Received().CreateAsync(dto);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ThenDelegatesToProperService()
    {
        await _sut.GetAllAsync();
        await _readService.Received().GetAllAsync();
    }

    [Fact]
    public async Task GetByChatIdAsync_WhenCalled_ThenDelegatesToProperService()
    {
        await _sut.GetByChatIdAsync(1);
        await _readService.Received().GetByChatIdAsync(1);
    }

    [Fact]
    public async Task GetListAsync_WhenCalled_ThenDelegatesToProperService()
    {
        var options = new PaginationRequest<GetUsersRequest>();
        await _sut.GetListAsync(options);
        await _readService.Received().GetListAsync(options);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalled_ThenDelegatesToProperService()
    {
        var userId = Guid.NewGuid();
        await _sut.GetByIdAsync(userId);
        await _readService.Received().GetByIdAsync(userId);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_ThenDelegatesToProperService()
    {
        var userId = Guid.NewGuid();
        var dto = new UserUpdateDto();
        await _sut.UpdateAsync(userId, dto);
        await _createUpdateService.Received().UpdateAsync(userId, dto);
    }

    [Fact]
    public async Task PartialUpdateAsync_WhenCalled_ThenDelegatesToProperService()
    {
        var userId = Guid.NewGuid();
        var dto = new UserPartialUpdateDto();
        await _sut.UpdatePartialAsync(userId, dto);
        await _createUpdateService.Received().UpdatePartialAsync(userId, dto);
    }
}
