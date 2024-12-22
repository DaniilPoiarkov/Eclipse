using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;

using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;

namespace Eclipse.Localization.Resources;

internal sealed class ResourceProvider : IResourceProvider
{
    private readonly ConcurrentDictionary<string, LocalizationResource> _resourceCache = [];

    private readonly ConcurrentDictionary<string, object?> _missingResources = [];

    private readonly IOptions<LocalizationBuilder> _options;

    private static readonly Lazy<JsonSerializerOptions> _serializerOptions = new(() => new()
    {
        PropertyNameCaseInsensitive = true,
    });

    public ResourceProvider(IOptions<LocalizationBuilder> options)
    {
        _options = options;
    }

    public LocalizationResource Get(CultureInfo culture)
    {
        return Get(culture.Name, culture.Name, _options.Value.Resources);
    }

    public LocalizationResource Get(CultureInfo culture, string location)
    {
        return Get(culture.Name, $"location={location};culture={culture.Name}", [location]);
    }

    private LocalizationResource Get(string cultureName, string key, IEnumerable<string> resources)
    {
        if (_resourceCache.TryGetValue(key, out var resource))
        {
            return resource;
        }

        if (_missingResources.ContainsKey(key))
        {
            if (_missingResources.ContainsKey(_options.Value.DefaultCulture))
            {
                throw new LocalizationFileNotExistException(string.Join(", ", resources), cultureName);
            }

            return Get(_options.Value.DefaultCulture, _options.Value.DefaultCulture, resources);
        }

        resource = ReadResources(resources)
            .FirstOrDefault(r => r.Culture == cultureName);

        if (resource is null)
        {
            _missingResources[key] = null;
            return Get(_options.Value.DefaultCulture, _options.Value.DefaultCulture, resources);
        }

        _resourceCache[key] = resource;

        return resource;
    }

    public LocalizationResource GetWithValue(string value)
    {
        var key = $"value={value}";

        if (_resourceCache.TryGetValue(key, out var resource))
        {
            return resource;
        }

        if (_missingResources.ContainsKey(key))
        {
            throw new LocalizationNotFoundException(value, nameof(value));
        }

        resource = _resourceCache.Values.FirstOrDefault(r => r.Texts.ContainsValue(value));

        if (resource is not null)
        {
            _resourceCache[key] = resource;
            return resource;
        }

        resource = ReadResources(_options.Value.Resources)
            .FirstOrDefault(r => r.Texts.ContainsValue(value));

        if (resource is null)
        {
            _missingResources[key] = null;
            throw new LocalizationNotFoundException(value, nameof(value));
        }

        return resource;
    }

    private static IEnumerable<LocalizationResource> ReadResources(params IEnumerable<string> resources)
    {
        return resources.Select(Path.GetFullPath)
            .SelectMany(path => path.EndsWith(".json")
                ? [path]
                : Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
            .Select(File.ReadAllText)
            .Select(json => JsonSerializer.Deserialize<LocalizationResource>(json, _serializerOptions.Value))
            .Where(r => r is not null)
            .GroupBy(r => r!.Culture)
            .Select(g => new LocalizationResource
            {
                Culture = g.Key,
                Texts = g.SelectMany(r => r!.Texts).ToDictionary()
            });
    }
}
