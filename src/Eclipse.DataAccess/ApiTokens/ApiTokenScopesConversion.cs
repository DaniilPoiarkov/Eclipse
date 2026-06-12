using Eclipse.Domain.Shared.ApiTokens;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Eclipse.DataAccess.ApiTokens;

internal sealed class ApiTokenScopesConversion : ValueConverter<IReadOnlyList<ApiTokenScope>, string[]>
{
    public ApiTokenScopesConversion() : base(
        scopes => scopes.Select(s => s.ToString()).ToArray(),
        names => names.Select(n => Enum.Parse<ApiTokenScope>(n)).ToArray()
    ) { }
}
