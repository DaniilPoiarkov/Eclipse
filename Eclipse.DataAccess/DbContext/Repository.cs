using Eclipse.Domain.Base;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.DbContext;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly IDbContext _context;

    protected Repository(IDbContext context)
    {
        _context = context;
    }

    public virtual void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public virtual void Delete(Guid id)
    {
        var set = _context.Set<TEntity>();
        var entity = set.FirstOrDefault(e => e.Id == id);

        if (entity is not null)
        {
            set.Remove(entity);
        }
    }

    public virtual IReadOnlyList<TEntity> GetAll() => _context.Set<TEntity>().AsReadOnly();

    public virtual IReadOnlyList<TEntity> GetByExpression(Expression<Func<TEntity, bool>> expression) =>
        _context.Set<TEntity>().Where(expression.Compile()).ToList();

    public virtual TEntity? GetById(Guid id) => _context.Set<TEntity>().FirstOrDefault(e => e.Id == id);

    public virtual void Update(TEntity entity) => _context.Update(entity);
}
