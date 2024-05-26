using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Users;

namespace Eclipse.DataAccess.Users;

internal sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(EclipseDbContext context)
        : base(context) { }
}
