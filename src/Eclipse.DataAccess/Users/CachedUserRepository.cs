using Eclipse.Common.Cache;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Users;

namespace Eclipse.DataAccess.Users;

internal sealed class CachedUserRepository : CachedRepositoryBase<User>, IUserRepository
{
    public CachedUserRepository(IUserRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }
}
