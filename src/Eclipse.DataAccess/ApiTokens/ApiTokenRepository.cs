using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.ApiTokens;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.ApiTokens;

internal sealed class ApiTokenRepository : RepositoryBase<ApiToken>, IApiTokenRepository
{
    public ApiTokenRepository(EclipseDbContext context)
        : base(context) { }

    public Task<ApiToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<ApiToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ApiToken>> GetExpiringBetweenAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.ExpiresAt >= from && t.ExpiresAt < to)
            .ToListAsync(cancellationToken);
    }
}
