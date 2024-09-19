using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Configuration;

[Serializable]
public sealed class CultureInfo
{
    [Required]
    public string Culture { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
