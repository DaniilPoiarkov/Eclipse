using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Eclipse.DataAccess.Repositories.Caching;

internal sealed class ExpressionHashStore : IExpressionHashStore
{
    private static readonly ClosureEvaluator _closureEvaluator = new();

    private static readonly ConcurrentDictionary<string, string> _hashes = [];

    public string Get<T>(Expression<Func<T, bool>> expression)
    {
        var evaluated = _closureEvaluator.Visit(expression);

        var body = evaluated.ToString();

        if (!_hashes.TryGetValue(body, out var hash))
        {
            hash = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(body))
            );

            _hashes.TryAdd(body, hash);
        }

        return hash;
    }
}
