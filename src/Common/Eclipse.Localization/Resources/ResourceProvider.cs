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

    private static readonly JsonSerializerOptions _serializationOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public ResourceProvider(IOptions<LocalizationBuilder> options)
    {
        _options = options;
    }

    public LocalizationResource Get(CultureInfo culture)
    {
        var cultureName = culture.Name;

        if (_resourceCache.TryGetValue(cultureName, out var resource))
        {
            return resource;
        }

        if (_missingResources.ContainsKey(cultureName))
        {
            if (_missingResources.ContainsKey(_options.Value.DefaultCulture))
            {
                throw new LocalizationFileNotExistException(string.Join(", ", _options.Value.Resources), cultureName);
            }

            return Get(CultureInfo.GetCultureInfo(_options.Value.DefaultCulture));
        }

        ReadAndCacheLocalizationResources();

        if (_resourceCache.TryGetValue(cultureName, out resource))
        {
            return resource;
        }

        _missingResources[cultureName] = null;

        if (_missingResources.ContainsKey(_options.Value.DefaultCulture))
        {
            throw new LocalizationFileNotExistException(string.Join(", ", _options.Value.Resources), cultureName);
        }

        return Get(CultureInfo.GetCultureInfo(_options.Value.DefaultCulture));
    }

    public LocalizationResource Get(CultureInfo culture, string location)
    {
        var cultureName = culture.Name;

        var key = $"location={location};culture={cultureName}";

        if (_resourceCache.TryGetValue(key, out var resource))
        {
            return resource;
        }

        if (_missingResources.ContainsKey(key))
        {
            throw new LocalizationFileNotExistException(location, cultureName);
        }

        var fullPath = Path.GetFullPath(location);

        if (fullPath.EndsWith(".json"))
        {
            return ParseAndCacheFile(location, key, fullPath);
        }

        resource = Directory.GetFiles(fullPath, "*.json", SearchOption.AllDirectories)
            .Select(File.ReadAllText)
            .Select(json => JsonSerializer.Deserialize<LocalizationResource>(json, _serializationOptions))
            .GroupBy(r => r!.Culture)
            .Select(g => new LocalizationResource
            {
                Culture = g.Key,
                Texts = g.SelectMany(r => r!.Texts).ToDictionary()
            })
            .FirstOrDefault(r => r.Culture == cultureName);

        if (resource is null)
        {
            _missingResources[key] = null;
            throw new LocalizationFileNotExistException(location, cultureName);
        }

        _resourceCache[key] = resource;

        return resource;
    }

    private LocalizationResource ParseAndCacheFile(string location, string key, string fullPath)
    {
        var json = File.ReadAllText(fullPath);

        var resource = JsonSerializer.Deserialize<LocalizationResource>(json, _serializationOptions);

        if (resource is null || string.IsNullOrEmpty(resource.Culture))
        {
            _missingResources[key] = null;
            throw new UnableToParseLocalizationResourceException(location);
        }

        _resourceCache[key] = resource;
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
            .SelectMany(path => path.EndsWith(".json")
                ? [path]
                : Directory.GetFiles(path, "*.json", SearchOption.AllDirectories))
            .Select(File.ReadAllText)
            .Select(json => JsonSerializer.Deserialize<LocalizationResource>(json, _serializationOptions))
            .Where(r => r is not null)
            .GroupBy(r => r!.Culture)
            .Select(g => new LocalizationResource
            {
                Culture = g.Key,
                Texts = g.SelectMany(r => r!.Texts).ToDictionary()
            });
    }
}
