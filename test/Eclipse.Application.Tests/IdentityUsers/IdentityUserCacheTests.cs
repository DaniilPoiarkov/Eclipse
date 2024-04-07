using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Common.Cache;
using Eclipse.Tests.Generators;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.IdentityUsers;

public sealed class IdentityUserCacheTests
{
    private readonly ICacheService _cacheService;

    private readonly Lazy<IIdentityUserCache> _lazySut;
    
    private readonly CacheKey _key;

    private IIdentityUserCache Sut => _lazySut.Value;

    public IdentityUserCacheTests()
    {
        _cacheService = Substitute.For<ICacheService>();
        _lazySut = new Lazy<IIdentityUserCache>(() => new IdentityUserCache(_cacheService));
        _key = new CacheKey("Identity-Users");
    }

    [Fact]
    public async Task GetAll_WhenUsersCached_ThenCachedUsersReturned()
    {
        var users = IdentityUserDtoGenerator.Generate(1, 5);

        _cacheService.GetAsync<List<IdentityUserDto>>(_key)!
            .ReturnsForAnyArgs(Task.FromResult(users));

        var result = await Sut.GetAllAsync();

        result.Count.Should().Be(users.Count);
        result.All(r => users.Exists(u => u.Id == r.Id)).Should().BeTrue();
        await _cacheService.ReceivedWithAnyArgs().GetAsync<List<IdentityUserDto>>(_key);
    }

    [Fact]
    public async Task AddOrUpdate_WhenUserNotCached_ThenAddUserToCachedCollection()
    {
        var cached = IdentityUserDtoGenerator.Generate(1, 4);
        var dto = IdentityUserDtoGenerator.Generate(5, 1).First();

        _cacheService.GetAsync<List<IdentityUserDto>>(_key)!
            .ReturnsForAnyArgs(Task.FromResult(cached));

        await Sut.AddOrUpdateAsync(dto);

        await _cacheService.ReceivedWithAnyArgs().GetAsync<List<IdentityUserDto>>(_key);
        cached.Add(dto);
        await _cacheService.ReceivedWithAnyArgs().SetAsync(_key, cached);
    }
}
