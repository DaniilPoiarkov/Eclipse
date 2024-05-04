using System.ComponentModel.DataAnnotations;

namespace Eclipse.WebAPI.Options;

[Serializable]
public sealed class LocalizationOptions
{
    [Required]
    public string DefaultCulture { get; set; } = null!;

    [Required]
    public string[] AvailableCultures { get; set; } = [];
}
