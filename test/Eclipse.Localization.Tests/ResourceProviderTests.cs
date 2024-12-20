using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class ResourceProviderTests
{
    private readonly LocalizationBuilder _builder;

    private readonly Lazy<IResourceProvider> _lazy;

    private IResourceProvider Sut => _lazy.Value;

    public ResourceProviderTests()
    {
        _builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        _lazy = new(() => new ResourceProvider(Options.Create(_builder)));
    }

    [Theory]
    [InlineData("empty.json")]
    [InlineData("invalid.json")]
    public void Get_WhenFileIsWrongFormat_ThenExceptionThrown(string file)
    {
        var action = () => Sut.Get(new("en"), $"Resources/Invalid/{file}");
        action.Should().ThrowExactly<UnableToParseLocalizationResourceException>();
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

        var expected = JsonConvert.DeserializeObject<LocalizationResource>(json);

        var resource = Sut.Get(new(culture), path);
        resource.Culture.Should().Be(culture);
        resource.Texts.Count.Should().Be(expected!.Texts.Count);
        resource.Texts.Except(expected.Texts).Should().BeEmpty();
    }

    [Theory]
    [InlineData("en", "Test", "Test")]
    [InlineData("uk", "Test", "Тест")]
    public void Get_WhenCultureSpecified_ThenReturnsResourceWithProperCulture(string culture, string key, string expected)
    {
        _builder.AddJsonFiles("Resources/Valid");

        var resource = Sut.Get(new(culture));
        resource.Culture.Should().Be(culture);

        var value = resource.Texts[key];
        value.Should().Be(expected);
    }

    [Theory]
    [InlineData("Test", "en", "Test")]
    [InlineData("Тест", "uk", "Test")]
    [InlineData("Message {0}", "en", "Message{0}")]
    public void GetWithValue_ThenResourceExist_ThenReturnsWithSpecifiedValue(string value, string expectedCulture, string expectedKey)
    {
        _builder.AddJsonFiles("Resources/Valid");

        var resource = Sut.GetWithValue(value);

        resource.Culture.Should().Be(expectedCulture);
        var pair = resource.Texts.FirstOrDefault(pair => pair.Value == value);
        pair.Key.Should().Be(expectedKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("localization test")]
    public void GetWithValue_ThenResourceNotExist_THenExceptionThrown(string value)
    {
        var action = () => Sut.GetWithValue(value);
        action.Should().ThrowExactly<LocalizationNotFoundException>();
    }
}
