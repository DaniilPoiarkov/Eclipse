using Microsoft.Extensions.Localization;

namespace Eclipse.Localization.Localizers;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource)
    {
        throw new NotImplementedException();
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        throw new NotImplementedException();
    }
}
