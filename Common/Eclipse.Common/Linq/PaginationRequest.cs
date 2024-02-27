using System.ComponentModel.DataAnnotations;

namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginationRequest<TOptions>
    where TOptions : class
{
    [Required]
    public TOptions Options { get; set; } = null!;

    [Range(1, 25)]
    public int PageSize { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; }
}
