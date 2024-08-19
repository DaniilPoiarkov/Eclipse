using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.Repositories;

public class RepositoryBase<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly EclipseDbContext Context;
    protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

    public RepositoryBase(EclipseDbContext context)
    {
        Context = context;
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(expression, cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity is null)
        {
            return;
        }

        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(expression)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(expression)
            .OrderBy(e => e.Id)
            .Skip(skipCount)
            .Take(takeCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);

        return entry.Entity;
    }
}
