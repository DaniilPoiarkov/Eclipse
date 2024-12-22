using Eclipse.Localization.Builder;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using System.Globalization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class JsonStringLocalizerFactoryTests
{
    private readonly JsonStringLocalizerFactory _sut;

    public JsonStringLocalizerFactoryTests()
    {
        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        builder.AddJsonFiles("Resources");

        var options = Options.Create(builder);

        _sut = new JsonStringLocalizerFactory(new ResourceProvider(options));
    }

    [Theory]
    [InlineData("en", "Test", "Test", false)]
    [InlineData("en", "Test1", "Test 1", false)]
    [InlineData("en", "Test2", "Test 2", false)]
    [InlineData("uk", "Test", "Тест", false)]
    [InlineData("uk", "Test3", "Test3", true)]
    [InlineData("uk", "Test4", "Test4", true)]
    public void Create_WhenLocalizerCreated_ThenItCanProperlyLocalizeStrings(string culture, string key, string value, bool resourceNotFound)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(culture);

        var localizer = _sut.Create(GetType());
        localizer[key].Should().BeEquivalentTo(new LocalizedString(key, value, resourceNotFound));
    }

    [Theory]
    [InlineData("en", "", "Resources", "Test", "Test", false)]
    [InlineData("en", "", "Resources", "Test1", "Test 1", false)]
    [InlineData("en", "", "Resources", "Test2", "Test 2", false)]
    [InlineData("uk", "", "Resources", "Test", "Тест", false)]
    [InlineData("uk", "", "Resources", "Test3", "Test3", true)]
    [InlineData("uk", "", "Resources", "Test4", "Test4", true)]
    public void Create_WhenLocationSpecified_ThenCanProperlyLocalizeStrings(string culture, string baseName, string location, string key, string value, bool resourceNotFound)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(culture);

        var localizer = _sut.Create(baseName, location);
        localizer[key].Should().BeEquivalentTo(new LocalizedString(key, value, resourceNotFound, location));
    }

    [Theory]
    [InlineData("en", "Test", "Test")]
    [InlineData("en", "Test1", "Test 1")]
    public void Create_WhenNoArgumentsPassed_ThenCanProduceLocalizer(string culture, string key, string value)
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

        var localizer = _sut.Create();
        localizer[key].Should().BeEquivalentTo(new LocalizedString(key, value));
    }
}
