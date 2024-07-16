using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Extensions;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NSubstitute;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class JsonStringLocalizerFactoryTests
{
    private readonly ICurrentCulture _currentCulture;

    private readonly Lazy<JsonStringLocalizerFactory> _sut;

    private JsonStringLocalizerFactory Sut => _sut.Value;

    public JsonStringLocalizerFactoryTests()
    {
        _currentCulture = Substitute.For<ICurrentCulture>();

        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        builder.AddJsonFiles("Resources");

        var options = Options.Create(builder);

        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        httpContextAccessor.HttpContext?.RequestServices
            .GetService(typeof(ICurrentCulture))
            .ReturnsForAnyArgs(_currentCulture);

        _sut = new(() => new JsonStringLocalizerFactory(options, new ResourceProvider(options), httpContextAccessor));
    }

    [Theory]
    [InlineData("en", "Test", "Test", false)]
    [InlineData("en", "Test1", "Test 1", false)]
    [InlineData("en", "Test2", "Test 2", false)]
    [InlineData("uk", "Test", "Тест", false)]
    [InlineData("uk", "Test3", "Test3", true)]
    [InlineData("uk", "Test4", "Test4", true)]
    public void Create_WhenLocalizerCreated_ThenItCanProperlyLocalizeStrings(string culture, string key, string expected, bool resourceNotFound)
    {
        _currentCulture.Culture.Returns(culture);

        var localizer = Sut.Create(GetType());

        var actual = localizer[key];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
    }

    [Theory]
    [InlineData("en", "", "Resources", "Test", "Test", false)]
    [InlineData("en", "", "Resources", "Test1", "Test 1", false)]
    [InlineData("en", "", "Resources", "Test2", "Test 2", false)]
    [InlineData("uk", "", "Resources", "Test", "Тест", false)]
    [InlineData("uk", "", "Resources", "Test3", "Test3", true)]
    [InlineData("uk", "", "Resources", "Test4", "Test4", true)]
    public void Create_WhenLocationSpecified_ThenCanProperlyLocalizeStrings(string culture, string baseName, string location, string key, string expected, bool resourceNotFound)
    {
        _currentCulture.Culture.Returns(culture);

        var localizer = Sut.Create(baseName, location);

        var actual = localizer[key];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
        actual.SearchedLocation.Should().Be(location);
    }






    [Theory]
    [InlineData("Test", "en", "Test", false)]
    [InlineData("Test1", "en", "Test 1", false)]
    [InlineData("Test2", "en", "Test 2", false)]
    [InlineData("Test", "uk", "Тест", false)]
    [InlineData("Test1", "uk", "Тест 1", false)]
    [InlineData("Test2", "uk", "Тест 2", false)]
    public void GetString_WhenCultureSpecified_ThenReturnsStringWithSpecifiedCulture(string key, string culture, string expected, bool resourceNotFound)
    {
        _currentCulture.Culture.Returns(culture);

        var str = Sut.Create()[key, culture];

        str.Name.Should().Be(key);
        str.Value.Should().Be(expected);
        str.ResourceNotFound.Should().Be(resourceNotFound);
    }
}
