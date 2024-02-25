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
        var specification = Specification<IdentityUser>.Empty;

        if (!name.IsNullOrEmpty())
        {
            specification = specification
                .And(new NameSpecification(name));
        }

        if (!userName.IsNullOrEmpty())
        {
            specification = specification
                .And(new UserNameSpecification(userName));
        }

        if (notificationEnabled)
        {
            specification = specification
                .And(new NotificationsEnabledSpecification());
        }

        return GetByExpressionAsync(specification, cancellationToken);
    }
}
