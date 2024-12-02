using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Eclipse.Localization;

internal sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IOptions<LocalizationBuilder> _options;

    private readonly IResourceProvider _resourceProvider;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public JsonStringLocalizerFactory(IOptions<LocalizationBuilder> options, IResourceProvider resourceProvider, IHttpContextAccessor httpContextAccessor)
    {
        _options = options;
        _resourceProvider = resourceProvider;
        _httpContextAccessor = httpContextAccessor;
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
        var currentCulture = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ICurrentCulture>()
            ?? new CurrentCulture(_options);

        return new JsonStringLocalizer(_resourceProvider, currentCulture, location);
    }
}
