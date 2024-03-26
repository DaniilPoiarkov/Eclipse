using Eclipse.DataAccess.EclipseCosmosDb;
using Eclipse.Domain.IdentityUsers;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.IdentityUsers;

internal sealed class NullIdentityUserRepository : IIdentityUserRepository
{
    private readonly EclipseCosmosDbContext _context;

    public NullIdentityUserRepository(EclipseCosmosDbContext context)
    {
        _context = context;
    }

    public Task<int> CountAsync(Expression<Func<IdentityUser, bool>> expression, CancellationToken cancellationToken = default)
    {
        return _context.IdentityUsers.CountAsync(expression, cancellationToken);
    }

    public async Task<IdentityUser> CreateAsync(IdentityUser entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IdentityUser?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.IdentityUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<IdentityUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.IdentityUsers.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IdentityUser>> GetByExpressionAsync(Expression<Func<IdentityUser, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _context.IdentityUsers.Where(expression).ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<IdentityUser>> GetByExpressionAsync(Expression<Func<IdentityUser, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default)
    {
        return GetByExpressionAsync(expression, cancellationToken);
    }

    public async Task<IdentityUser> UpdateAsync(IdentityUser entity, CancellationToken cancellationToken = default)
    {
        var entry = _context.IdentityUsers.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }
}
