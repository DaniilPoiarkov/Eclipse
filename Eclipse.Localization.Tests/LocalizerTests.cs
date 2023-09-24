using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Localization.Tests;

public class LocalizerTests
{
    private readonly ILocalizer _sut = GetLocalizer();

    [Fact]
    public void Localize_WhenCanBeLocalized_ThenLocalizedValueReturned()
    {
        var key = "Test";
        var localizedAsEn = _sut[key, "en"];
        var localizedAsUk = _sut[key, "uk"];

        localizedAsEn.Should().Be("Test");
        localizedAsUk.Should().Be("Тест");
    }

    [Fact]
    public void Localize_WhenLocalizationNotExist_ThenDefaultLocalizationUsed()
    {
        var localized = _sut["Test", "fr"];
        localized.Should().Be("Test");
    }

    [Fact]
    public void ToLocalizableString_WhenLocalizationExist_ThenKeyReturned()
    {
        var localizedAsEn = "Test";
        var localizedAsUk = "Тест";

        var keyFromEnLocalization = _sut.ToLocalizableString(localizedAsEn);
        var keyFromUkLocalization = _sut.ToLocalizableString(localizedAsUk);

        keyFromEnLocalization.Should().Be(keyFromUkLocalization);
    }

    [Fact]
    public void ToLocalizableString_WhenLocalizationNotExist_ThenExceptionThrown()
    {
        var value = "Prüfen";

        var action = () => _sut.ToLocalizableString(value);

        action.Should().Throw<LocalizationNotFoundException>();
    }

    [Fact]
    public void FormatLocalizableException_WhenCanBeFormatted_ThenShouldReturnFormattedString()
    {
        var exception = new LocalizedException("ExMessage", "ExArg");
        var localized = _sut.FormatLocalizedException(exception, "en");

        localized.Should().Be("Exception ExArg");
    }

    private static ILocalizer GetLocalizer()
    {
        var localizationDirectory = "Localization";

        return new LocalizationBuilder()
            .AddJsonFile($"{localizationDirectory}/en.json")
            .AddJsonFile($"{localizationDirectory}/uk.json")
            .Build();
    }
}
