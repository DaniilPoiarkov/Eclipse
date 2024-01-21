using Eclipse.Localization.Localizers;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Localization.Builder;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IOptions<LocalizationOptions> _options;

    private readonly ResourceManagerStringLocalizerFactory _innerFactory;

    public JsonStringLocalizerFactory(
        IOptions<LocalizationOptions> options,
        ResourceManagerStringLocalizerFactory innerFactory)
    {
        _options = options;
        _innerFactory = innerFactory;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        var resource = _options.Value.Resources.FirstOrDefault(r => r.Type == resourceSource);

        if (resource is null)
        {
            return _innerFactory.Create(resourceSource);
        }

        // TODO: typed exception
        var culture = resource.GetCultureInfo("")
            ?? throw new Exception("MAKE BETTER EXCEPTION");

        return new JsonStringLocalizer(culture);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return _innerFactory.Create(baseName, location);
    }
}
