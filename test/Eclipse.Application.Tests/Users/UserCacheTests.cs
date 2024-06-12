using Eclipse.Application.Caching;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Users;
using Eclipse.Common.Cache;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Users;

public sealed class UserCacheTests
{
    private readonly ICacheService _cacheService;

    private readonly Lazy<IUserCache> _lazySut;

    private readonly CacheKey _key;

    private IUserCache Sut => _lazySut.Value;

    public UserCacheTests()
    {
        _cacheService = Substitute.For<ICacheService>();
        _lazySut = new Lazy<IUserCache>(() => new UserCache(_cacheService));
        _key = new CacheKey("Users");
    }

    [Fact]
    public async Task GetAll_WhenUsersCached_ThenCachedUsersReturned()
    {
        var users = UserDtoGenerator.Generate(1, 5);

        _cacheService.GetAsync<List<UserDto>>(_key)!
            .ReturnsForAnyArgs(Task.FromResult(users));

        var result = await Sut.GetAllAsync();

        result.Count.Should().Be(users.Count);
        result.All(r => users.Exists(u => u.Id == r.Id)).Should().BeTrue();
        await _cacheService.ReceivedWithAnyArgs().GetAsync<List<UserDto>>(_key);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotCached_ThenAddUserToCachedCollection()
    {
        var cached = UserDtoGenerator.Generate(1, 4);
        var dto = UserDtoGenerator.Generate(5, 1).First();

        _cacheService.GetAsync<List<UserDto>>(_key)!
            .ReturnsForAnyArgs(Task.FromResult(cached));

        await Sut.AddOrUpdateAsync(dto);

        await _cacheService.ReceivedWithAnyArgs().GetAsync<List<UserDto>>(_key);
        cached.Add(dto);
        await _cacheService.ReceivedWithAnyArgs().SetAsync(_key, cached, CacheConsts.OneDay);
    }
}
