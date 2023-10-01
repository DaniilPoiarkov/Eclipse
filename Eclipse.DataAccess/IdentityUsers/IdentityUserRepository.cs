using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.IdentityUsers;

namespace Eclipse.DataAccess.IdentityUsers;

internal class IdentityUserRepository : CosmosRepository<IdentityUser>, IIdentityUserRepository
{
    public IdentityUserRepository(EclipseCosmosDbContext context) : base(context.IdentityUsers)
    {
    }
}
