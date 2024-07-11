using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using FluentAssertions;

using Xunit;

namespace Eclipse.Localization.Tests;

public class LocalizerTests
{
    private readonly ILocalizer _sut = GetLocalizer();

    [Theory]
    [InlineData("Test", "en", "Test")]
    [InlineData("Test", "uk", "Тест")]
    public void Localize_WhenCanBeLocalized_ThenLocalizedValueReturned(string key, string culture, string expectedResult)
    {
        var localizedAsEn = _sut[key, culture];
        localizedAsEn.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void Localize_WhenLocalizationNotExist_ThenDefaultLocalizationUsed()
    {
        var localized = _sut["Test", "fr"];
        localized.Value.Should().Be("Test");
    }

    [Theory]
    [InlineData("Test", "Тест")]
    [InlineData("Test 1", "Тест 1")]
    [InlineData("Test 2", "Тест 2")]
    public void ToLocalizableString_WhenLocalizationExist_ThenKeyReturned(string leftLocalized, string rightLocalized)
    {
        var keyFromLeft = _sut.ToLocalizableString(leftLocalized);
        var keyFromRight = _sut.ToLocalizableString(rightLocalized);

        keyFromLeft.Should().Be(keyFromRight);
    }

    [Fact]
    public void ToLocalizableString_WhenLocalizationNotExist_ThenExceptionThrown()
    {
        var value = "Prüfen";

        var action = () => _sut.ToLocalizableString(value);

        action.Should().Throw<LocalizationNotFoundException>();
    }

    [Theory]
    [InlineData("en", "Exception Arg")]
    [InlineData("uk", "Помилка Arg")]
    public void FormatLocalizableException_WhenCanBeFormatted_ThenShouldReturnFormattedString(string culture, string expectedResult)
    {
        var exception = new LocalizedException("ExceptionMessage", "Arg");
        var localized = _sut.FormatLocalizedException(exception, culture);

        localized.Should().Be(expectedResult);
    }

    private static ILocalizer GetLocalizer()
    {
        var builder = new LocalizationBuilder();

        builder.AddJsonFiles("Resources/Valid");

        return builder.Build();
    }
}
