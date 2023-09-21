using Eclipse.Localization.Localizers;

namespace Eclipse.Localization.Builder;

public interface ILocalizationBuilder
{
    /// <summary>
    /// Add json localization file 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    ILocalizationBuilder AddJsonFile(string path);

    string DefaultLocalization { get; set; }

    ILocalizer Build();
}
