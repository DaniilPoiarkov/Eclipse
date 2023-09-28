using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.InMemoryDb;

public abstract class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly IDbContext _context;

    protected InMemoryRepository(IDbContext context)
    {
        _context = context;
    }

    public virtual Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Add(entity);
        return Task.FromResult<TEntity?>(entity);
    }

    public virtual Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        return Task.FromResult<TEntity?>(entity);
    }

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var set = _context.Set<TEntity>();
        var entity = set.FirstOrDefault(e => e.Id == id);

        if (entity is not null)
        {
            set.Remove(entity);
        }

        return Task.CompletedTask;
    }

    public virtual Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TEntity>>(_context.Set<TEntity>().AsReadOnly());
    }

    public virtual Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        var result = _context.Set<TEntity>().Where(expression.Compile()).ToList();
        return Task.FromResult<IReadOnlyList<TEntity>>(result.AsReadOnly());
    }

    public virtual Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_context.Set<TEntity>().FirstOrDefault(e => e.Id == id));
    }
}
