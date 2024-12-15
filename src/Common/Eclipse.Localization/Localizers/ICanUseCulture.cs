using Eclipse.Localization.Culture;

namespace Eclipse.Localization.Localizers;

public interface ICanUseCulture
{
    void UseCurrentCulture(ICurrentCulture currentCulture);
}
