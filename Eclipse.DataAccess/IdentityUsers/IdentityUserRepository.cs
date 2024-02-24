using Eclipse.DataAccess.CosmosDb;
using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.IdentityUsers.Specifications;
using Eclipse.Domain.Shared.Specifications;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.DataAccess.IdentityUsers;

internal sealed class IdentityUserRepository : CosmosRepository<IdentityUser>, IIdentityUserRepository
{
    public IdentityUserRepository(EclipseCosmosDbContext context, IServiceProvider serviceProvider)
        : base(context.IdentityUsers, serviceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IPublisher>()) { }

    public Task<IReadOnlyList<IdentityUser>> GetByFilterAsync(string? name, string? userName, bool notificationEnabled, CancellationToken cancellationToken = default)
    {
        var specification = new NameSpecification(name)
            .And(new UserNameSpecification(userName))
            .And(new NotificationsEnabledSpecification(notificationEnabled));

        return GetByExpressionAsync(specification, cancellationToken);
    }
}
