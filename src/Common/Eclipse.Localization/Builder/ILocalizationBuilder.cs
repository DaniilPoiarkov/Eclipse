namespace Eclipse.Localization.Builder;

public interface ILocalizationBuilder
{
    /// <summary>
    /// Adds json files in specified and all child directories. Multiple resource files with same culture will be combined all together.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    ILocalizationBuilder AddJsonFiles(string path);

    /// <summary>
    /// Default fallback culture.
    /// </summary>
    string DefaultCulture { get; set; }
}
