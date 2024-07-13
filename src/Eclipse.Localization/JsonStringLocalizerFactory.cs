using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Localization;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory, ILocalizerFactory
{
    private readonly IOptions<LocalizationBuilderV2> _options;

    private readonly IResourceProvider _resourceProvider;

    private ICurrentCulture? CurrentCulture { get; set; }

    public JsonStringLocalizerFactory(IOptions<LocalizationBuilderV2> options, IResourceProvider resourceProvider)
    {
        _options = options;
        _resourceProvider = resourceProvider;
        CurrentCulture = new CurrentCulture(options);
    }

    internal JsonStringLocalizerFactory(
        IOptions<LocalizationBuilderV2> options,
        IResourceProvider resourceProvider,
        ICurrentCulture? currentCulture)
        : this(options, resourceProvider)
    {
        CurrentCulture = currentCulture;
    }

    internal void SetCurrentCulture(ICurrentCulture currentCulture)
    {
        CurrentCulture = currentCulture;
    }

    public ILocalizer Create()
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
        return new JsonStringLocalizer(_options, _resourceProvider, CurrentCulture ?? new CurrentCulture(_options), location);
    }
}
