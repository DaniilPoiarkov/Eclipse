using System.ComponentModel.DataAnnotations;

namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginationRequest<TOptions>
    where TOptions : class, new()
{
    [Required]
    public TOptions Options { get; init; } = new();

    [Range(1, 25)]
    public int PageSize { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; }
}
