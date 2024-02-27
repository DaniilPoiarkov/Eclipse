using System.ComponentModel.DataAnnotations;

namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginationRequest<TOptions>
    where TOptions : class
{
    [Required]
    public TOptions Options { get; set; } = null!;

    public int PageSize { get; set; }

    public int Page { get; set; }
}
