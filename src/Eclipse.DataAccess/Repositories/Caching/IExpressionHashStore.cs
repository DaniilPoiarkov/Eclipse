using System.Linq.Expressions;

namespace Eclipse.DataAccess.Repositories.Caching;

internal interface IExpressionHashStore
{
    string Get<T>(Expression<Func<T, bool>> expression);
}
