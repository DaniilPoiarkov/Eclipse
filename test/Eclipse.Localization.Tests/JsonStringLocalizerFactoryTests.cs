using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using FluentAssertions;

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

        var builder = new LocalizationBuilderV2
        {
            DefaultCulture = "en"
        };

        builder.AddJsonFiles("Resources");

        var options = Options.Create(builder);

        _sut = new(() => new JsonStringLocalizerFactory(options, _currentCulture, new ResourceProvider(options)));
    }

    #region IStringLocalizerFactory Tests

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

    #endregion

    #region ILocalizer Tests

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

        var str = Sut[key, culture];

        str.Name.Should().Be(key);
        str.Value.Should().Be(expected);
        str.ResourceNotFound.Should().Be(resourceNotFound);
    }

    [Theory]
    [InlineData("Test", "de")]
    [InlineData("Test", "fr")]
    public void GetString_WhenCultureNotExist_ThenExceptionThrown(string key, string culture)
    {
        _currentCulture.Culture.Returns(culture);

        var action = () => Sut[key, culture];
        action.Should().ThrowExactly<LocalizationFileNotExistException>();
    }

    [Theory]
    [InlineData("en", "ExceptionMessage", "Exception arg", "arg")]
    [InlineData("en", "{0}Message{1}", "Arg message 3", "Arg", 3)]
    [InlineData("uk", "ExceptionMessage", "Помилка 5", 5)]
    [InlineData("uk", "{0}Message{1}", "5 повідомлення false", 5, "false")]
    public void FormatLocalizableException_WhenCalled_ThenLocalizeMessageWithSpecifiedArguments(string culture, string template, string expected, params object[] arguments)
    {
        _currentCulture.Culture.Returns(culture);

        var exception = new LocalizedException(template, arguments);

        var actual = Sut.FormatLocalizedException(exception);

        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Test", "Test")]
    [InlineData("Test 1", "Test1")]
    [InlineData("Test 2", "Test2")]
    [InlineData("Тест", "Test")]
    [InlineData("Тест 1", "Test1")]
    [InlineData("Тест 2", "Test2")]
    public void ToLocalizableString_WhenCalled_ThenReturnsProperKey(string value, string expected)
    {
        var key = Sut.ToLocalizableString(value);
        key.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Prüfen")]
    [InlineData("Message with no information provided")]
    [InlineData("!@#5569-=-03123")]
    public void ToLocalizableString_WhenLocalizationNotFound_ThenExceptionThrown(string value)
    {
        var action = () => Sut.ToLocalizableString(value);
        action.Should().ThrowExactly<LocalizationNotFoundException>();
    }

    #endregion
}
