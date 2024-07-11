namespace Eclipse.Localization.Builder;

public interface ILocalizationBuilder
{
    /// <summary>
    /// Add json localization file. If specified localization already exists file content will expand specified localization
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    ILocalizationBuilder AddJsonFile(string path);

    /// <summary>
    /// Add json files in specified directory. Multiple localization files with same culture will combine all toghether
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    ILocalizationBuilder AddJsonFiles(string path);

    /// <summary>
    /// Default localization if localization culture not defined
    /// </summary>
    string DefaultCulture { get; set; }
}
