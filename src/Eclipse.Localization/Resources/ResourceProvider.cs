using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System.Collections.Concurrent;

namespace Eclipse.Localization.Resources;

internal sealed class ResourceProvider : IResourceProvider
{
    private readonly ConcurrentDictionary<string, LocalizationResource> _resourceCache = [];

    private readonly ConcurrentDictionary<string, object?> _missingResources = [];

    private readonly IOptions<LocalizationBuilderV2> _options;

    public ResourceProvider(IOptions<LocalizationBuilderV2> options)
    {
        _options = options;
    }

    public LocalizationResource Get(string culture)
    {
        if (_resourceCache.TryGetValue(culture, out var resource))
        {
            return resource;
        }

        if (_missingResources.ContainsKey(culture))
        {
            throw new LocalizationFileNotExistException(string.Join(", ", _options.Value.Resources), culture);
        }

        ReadAndCacheLocalizationResources();
        
        if (!_resourceCache.TryGetValue(culture, out resource))
        {
            _missingResources[culture] = null;
            throw new LocalizationFileNotExistException(string.Join(", ", _options.Value.Resources), culture);
        }

        return resource;
    }

    public LocalizationResource Get(string culture, string location)
    {
        var key = $"location={location};culture={culture}";

        if (!_resourceCache.TryGetValue(key, out var resource))
        {
            if (_missingResources.ContainsKey(key))
            {
                throw new LocalizationFileNotExistException(location, culture);
            }

            var fullPath = Path.GetFullPath(location);

            resource = Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories)
                .Select(File.ReadAllText)
                .Select(JsonConvert.DeserializeObject<LocalizationResource>)
                .GroupBy(r => r!.Culture)
                .Select(g => new LocalizationResource
                {
                    Culture = g.Key,
                    Texts = g.SelectMany(r => r!.Texts).ToDictionary()
                })
                .FirstOrDefault(r => r.Culture == culture);

            if (resource is null)
            {
                _missingResources[key] = null;
                throw new LocalizationFileNotExistException(location, culture);
            }

            _resourceCache[key] = resource;
        }

        return resource;
    }

    public LocalizationResource GetWithValue(string value)
    {
        if (_missingResources.ContainsKey(value))
        {
            throw new LocalizationNotFoundException(value, nameof(value));
        }

        var resource = GetFromCacheByValue(value);

        if (resource is not null)
        {
            return resource;
        }

        ReadAndCacheLocalizationResources();

        resource = GetFromCacheByValue(value);

        if (resource is null)
        {
            _missingResources[value] = null;
            throw new LocalizationNotFoundException(value, nameof(value));
        }

        return resource;
    }

    private LocalizationResource GetFromCacheByValue(string value)
    {
        return _resourceCache.FirstOrDefault(pair => pair.Value.Texts.Any(t => t.Value == value)).Value;
    }

    private void ReadAndCacheLocalizationResources()
    {
        var resources = ReadResources();
        CacheLocalizers(resources);
    }

    private void CacheLocalizers(IEnumerable<LocalizationResource> resources)
    {
        foreach (var resource in resources)
        {
            if (_missingResources.ContainsKey(resource.Culture))
            {
                _missingResources.Remove(resource.Culture, out _);
            }

            if (_resourceCache.ContainsKey(resource.Culture))
            {
                continue;
            }

            _resourceCache[resource.Culture] = resource;
        }
    }

    private IEnumerable<LocalizationResource> ReadResources()
    {
        return _options.Value.Resources
            .Select(Path.GetFullPath)
            .SelectMany(path => Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<LocalizationResource>)
            .Where(r => r is not null)
            .GroupBy(r => r!.Culture)
            .Select(g => new LocalizationResource
            {
                Culture = g.Key,
                Texts = g.SelectMany(r => r!.Texts).ToDictionary()
            });
    }
}
