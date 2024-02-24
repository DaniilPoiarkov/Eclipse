using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.IdentityUsers;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.DataAccess.IdentityUsers;

internal sealed class IdentityUserRepository : CosmosRepository<IdentityUser>, IIdentityUserRepository
{
    public IdentityUserRepository(EclipseCosmosDbContext context, IServiceProvider serviceProvider)
        : base(context.IdentityUsers, serviceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IPublisher>()) { }

    public Task<List<IdentityUser>> GetByFilterAsync(string? name, string? userName, bool notificationEnabled, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
