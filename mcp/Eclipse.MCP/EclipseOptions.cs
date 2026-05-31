using System.ComponentModel.DataAnnotations;

internal sealed class EclipseOptions
{
    [Required]
    public required string Url { get; init; }
}
