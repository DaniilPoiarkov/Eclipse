using Eclipse.Localization.Builder;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Options;

using System.Globalization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class JsonStringLocalizerFactoryTests
{
    private readonly Lazy<JsonStringLocalizerFactory> _sut;

    private JsonStringLocalizerFactory Sut => _sut.Value;

    public JsonStringLocalizerFactoryTests()
    {
        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en-US"
        };

        builder.AddJsonFiles("Resources");

        var options = Options.Create(builder);

        _sut = new(() => new JsonStringLocalizerFactory(new ResourceProvider(options)));
    }

    [Theory]
    [InlineData("en-US", "Test", "Test", false)]
    [InlineData("en-US", "Test1", "Test 1", false)]
    [InlineData("en-US", "Test2", "Test 2", false)]
    [InlineData("uk-UA", "Test", "Тест", false)]
    [InlineData("uk-UA", "Test3", "Test3", true)]
    [InlineData("uk-UA", "Test4", "Test4", true)]
    public void Create_WhenLocalizerCreated_ThenItCanProperlyLocalizeStrings(string culture, string key, string expected, bool resourceNotFound)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(culture);

        var localizer = Sut.Create(GetType());

        var actual = localizer[key];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
    }

    [Theory]
    [InlineData("en-US", "", "Resources", "Test", "Test", false)]
    [InlineData("en-US", "", "Resources", "Test1", "Test 1", false)]
    [InlineData("en-US", "", "Resources", "Test2", "Test 2", false)]
    [InlineData("uk-UA", "", "Resources", "Test", "Тест", false)]
    [InlineData("uk-UA", "", "Resources", "Test3", "Test3", true)]
    [InlineData("uk-UA", "", "Resources", "Test4", "Test4", true)]
    public void Create_WhenLocationSpecified_ThenCanProperlyLocalizeStrings(string culture, string baseName, string location, string key, string expected, bool resourceNotFound)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(culture);

        var localizer = Sut.Create(baseName, location);

        var actual = localizer[key];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
        actual.SearchedLocation.Should().Be(location);
    }
}
