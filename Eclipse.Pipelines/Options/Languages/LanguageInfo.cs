using System.ComponentModel.DataAnnotations;

namespace Eclipse.Pipelines.Options.Languages;

[Serializable]
public sealed class LanguageInfo
{
    [Required]
    public string Language { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
