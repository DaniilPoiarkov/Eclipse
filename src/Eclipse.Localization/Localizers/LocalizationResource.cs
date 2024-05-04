using System.Diagnostics;

namespace Eclipse.Localization.Localizers;

[DebuggerDisplay("{Culture}")]
[Serializable]
internal sealed class LocalizationResource
{
    public string Culture { get; set; } = string.Empty;

    public Dictionary<string, string> Texts { get; set; } = [];
}
