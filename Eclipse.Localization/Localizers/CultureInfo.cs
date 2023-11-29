using System.Diagnostics;

namespace Eclipse.Localization.Localizers;

[DebuggerDisplay("{Localization}")]
[Serializable]
internal sealed class CultureInfo
{
    public string Localization { get; set; } = string.Empty;

    public Dictionary<string, string> Texts { get; set; } = [];
}
