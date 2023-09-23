namespace Eclipse.Localization.Localizers;

internal class CultureInfo
{
    public string Localization { get; set; } = string.Empty;

    public Dictionary<string, string> Texts { get; set; } = new();
}
