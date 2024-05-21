using System.ComponentModel.DataAnnotations;

namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginationRequest<TOptions>
    where TOptions : class, new()
{
    [Required]
    public TOptions Options { get; set; } = new();

    [Range(1, 25)]
    public int PageSize { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; }
}
