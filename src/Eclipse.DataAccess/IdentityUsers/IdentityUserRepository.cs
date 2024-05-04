using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.DataAccess.IdentityUsers;

internal sealed class IdentityUserRepository : RepositoryBase<IdentityUser>, IIdentityUserRepository
{
    public IdentityUserRepository(EclipseDbContext context)
        : base(context) { }
}
