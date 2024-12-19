namespace Eclipse.Localization.Localizers;

public interface ICanUseCulture
{
    IDisposable UsingCulture(string culture);
}
