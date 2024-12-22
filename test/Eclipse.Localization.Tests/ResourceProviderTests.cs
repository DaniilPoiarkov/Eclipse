using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Options;

using System.Globalization;
using System.Text.Json;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class ResourceProviderTests
{
    private readonly LocalizationBuilder _builder;

    private readonly ResourceProvider _sut;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public ResourceProviderTests()
    {
        _builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        _sut = new ResourceProvider(Options.Create(_builder));
    }

    [Theory]
    [InlineData("empty.json")]
    [InlineData("invalid.json")]
    public void Get_WhenFileIsWrongFormat_ThenExceptionThrown(string file)
    {
        var action = () => _sut.Get(new CultureInfo("en"), $"Resources/Invalid/{file}");
        action.Should().ThrowExactly<LocalizationFileNotExistException>();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("uk")]
    public void Get_WhenNoFilesProvided_ThenThrowsException(string culture)
    {
        var action = () => _sut.Get(CultureInfo.GetCultureInfo(culture));
        action.Should().ThrowExactly<LocalizationFileNotExistException>();
    }

    [Theory]
    [InlineData("en", "en.json")]
    [InlineData("en", "en-1.json")]
    [InlineData("uk", "uk.json")]
    [InlineData("uk", "uk-1.json")]
    public void Get_WhenLocationProperlySpecified_ThenResourceReturned(string culture, string file)
    {
        var path = $"Resources/Valid/{file}";
        var json = File.ReadAllText(Path.GetFullPath(path));

        var expected = JsonSerializer.Deserialize<LocalizationResource>(json, _serializerOptions);

        var resource = _sut.Get(new CultureInfo(culture), path);

        resource.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("en", "Test", "Test")]
    [InlineData("uk", "Test", "Тест")]
    public void Get_WhenCultureSpecified_ThenReturnsProperResource(string culture, string key, string expected)
    {
        _builder.AddJsonFiles("Resources/Valid");

        var resource = _sut.Get(new CultureInfo(culture));

        resource.Culture.Should().Be(culture);
        resource.Texts[key].Should().Be(expected);
    }

    [Theory]
    [InlineData("fr")]
    [InlineData("de")]
    public void Get_WhenCultureNotExists_ThenReturnsWithDefaultCulture(string culture)
    {
        _builder.AddJsonFiles("Resources/Valid");

        var cultureInfo = CultureInfo.GetCultureInfo(culture);

        _ = _sut.Get(cultureInfo);

        var result = _sut.Get(cultureInfo);
        result.Culture.Should().Be(_builder.DefaultCulture);
    }

    [Theory]
    [InlineData("Test", "en", "Test")]
    [InlineData("Тест", "uk", "Test")]
    [InlineData("Message {0}", "en", "Message{0}")]
    public void GetWithValue_ThenResourceExist_ThenReturnsWithSpecifiedValue(string value, string culture, string key)
    {
        _builder.AddJsonFiles("Resources/Valid");

        var resource = _sut.GetWithValue(value);

        resource.Culture.Should().Be(culture);
        resource.Texts[key].Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("localization test")]
    public void GetWithValue_ThenResourceNotExist_THenExceptionThrown(string value)
    {
        var action = () => _sut.GetWithValue(value);
        action.Should().ThrowExactly<LocalizationNotFoundException>();
    }

    [Theory]
    [InlineData("Test", "en", "Test")]
    [InlineData("Тест", "uk", "Test")]
    public void GetWithValue_WhenResourceCached_ThenReturnsCachedValue(string value, string culture, string key)
    {
        _builder.AddJsonFiles("Resources/Valid");

        _ = _sut.GetWithValue(value);

        var resource = _sut.GetWithValue(value);
        resource.Culture.Should().Be(culture);
        resource.Texts[key].Should().Be(value);
    }

    [Theory]
    [InlineData("x")]
    public void GetWithValue_WhenCachedAsMissingResource_ThenThrowsException(string value)
    {
        try
        {
            _sut.GetWithValue(value);
        }
        catch (LocalizationNotFoundException)
        {

        }

        var action = () => _sut.GetWithValue(value);
        action.Should().ThrowExactly<LocalizationNotFoundException>();
    }

    [Theory]
    [InlineData("en", "Test")]
    public void GetWithValue_WhenResourceWasCached_ThenReturnsIt(string culture, string value)
    {
        _builder.AddJsonFiles("Resources/Valid");

        _ = _sut.Get(CultureInfo.GetCultureInfo(culture));

        var result = _sut.GetWithValue(value);
        result.Culture.Should().Be(culture);
    }
}
