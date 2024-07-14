using Eclipse.Localization.Exceptions;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Eclipse.Localization.Tests;

public class LocalizerTests
{
    private readonly ILocalizer _sut;

    private readonly IServiceProvider _serviceProvider;

    public LocalizerTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddLocalization(b =>
            {
                b.DefaultCulture = "en";
                b.AddJsonFiles("Resources/Valid");
            })
            .BuildServiceProvider();

        _sut = _serviceProvider.GetRequiredService<ILocalizer>();
    }

    [Theory]
    [InlineData("Test", "en", "Test")]
    [InlineData("Test", "uk", "Тест")]
    public void Localize_WhenCanBeLocalized_ThenLocalizedValueReturned(string key, string culture, string expectedResult)
    {
        var localizedAsEn = _sut[key, culture];
        localizedAsEn.Value.Should().Be(expectedResult);
    }

    [Fact]
    public void Localize_WhenLocalizationNotExist_ThenExceptionThrown()
    {
        var action = () => _sut["Test", "fr"];
        action.Should().ThrowExactly<LocalizationFileNotExistException>();
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

        action.Should().ThrowExactly<LocalizationNotFoundException>();
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
}
