using System.ComponentModel.DataAnnotations;

internal sealed class EclipseOptions
{
    [Required]
    public required string StandardUrl { get; init; }

    [Required]
    public required string TestingUrl { get; init; }
}
