using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IResourceProvider _resourceProvider;

    public JsonStringLocalizerFactory(IResourceProvider resourceProvider)
    {
        _resourceProvider = resourceProvider;
    }

    public IStringLocalizer Create()
    {
        return CreateJsonLocalizer();
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return CreateJsonLocalizer();
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return CreateJsonLocalizer(location);
    }

    private JsonStringLocalizer CreateJsonLocalizer(string? location = null)
    {
        return new JsonStringLocalizer(_resourceProvider, location);
    }
}
