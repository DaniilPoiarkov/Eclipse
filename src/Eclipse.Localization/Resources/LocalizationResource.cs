using System.Diagnostics;

namespace Eclipse.Localization.Resources;

[Serializable]
[DebuggerDisplay("{Culture}: {Texts.Count}")]
internal sealed class LocalizationResource
{
    public string Culture { get; set; } = string.Empty;

    public Dictionary<string, string> Texts { get; set; } = [];
}
