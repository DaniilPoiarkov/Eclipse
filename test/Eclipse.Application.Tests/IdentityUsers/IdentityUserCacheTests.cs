using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Common.Cache;
using Eclipse.Infrastructure.Cache;
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
    public void GetAll_WhenUsersCached_ThenCachedUsersReturned()
    {
        var users = IdentityUserDtoGenerator.GenerateUsers(1, 5);

        _cacheService.Get<List<IdentityUserDto>>(_key).ReturnsForAnyArgs(users);

        var result = Sut.GetAll();

        result.Count.Should().Be(users.Count);
        result.All(r => users.Exists(u => u.Id == r.Id)).Should().BeTrue();
        _cacheService.ReceivedWithAnyArgs().Get<List<IdentityUserDto>>(_key);
    }

    [Fact]
    public void AddOrUpdate_WhenUserNotCached_ThenAddUserToCachedCollection()
    {
        var cached = IdentityUserDtoGenerator.GenerateUsers(1, 4);
        var dto = IdentityUserDtoGenerator.GenerateUsers(5, 1).First();

        _cacheService.Get<List<IdentityUserDto>>(_key).ReturnsForAnyArgs(cached);

        Sut.AddOrUpdate(dto);

        _cacheService.ReceivedWithAnyArgs().Get<List<IdentityUserDto>>(_key);
        cached.Add(dto);
        _cacheService.ReceivedWithAnyArgs().Set(_key, cached);
    }
}
