using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.DataAccess.IdentityUsers;

internal sealed class EFCoreIdentityUserRepository : RepositoryBase<IdentityUser>, IIdentityUserRepository
{
    public EFCoreIdentityUserRepository(EclipseDbContext context)
        : base(context) { }
}
