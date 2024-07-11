namespace Eclipse.Localization.Resources;

internal interface IResourceProvider
{
    LocalizationResource Get(string culture);

    LocalizationResource Get(string culture, string location);

    LocalizationResource GetWithValue(string value);
}
