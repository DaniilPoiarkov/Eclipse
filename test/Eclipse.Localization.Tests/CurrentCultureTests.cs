using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CurrentCultureTests
{
    private readonly TypedJsonStringLocalizer<CurrentCultureTests> _localizer;

    private readonly CurrentCulture _sut;

    public CurrentCultureTests()
    {
        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en-US"
        };

        builder.AddJsonFiles("Resources");

        _sut = new CurrentCulture();

        var resourceProvider = new ResourceProvider(Options.Create(builder));
        var factory = new JsonStringLocalizerFactory(resourceProvider);

        _localizer = new TypedJsonStringLocalizer<CurrentCultureTests>(factory);
    }

    [Theory]
    [InlineData("Test", "uk-UA", "Test", "Тест")]
    [InlineData("Test1", "uk-UA", "Test 1", "Тест 1")]
    [InlineData("Test2", "uk-UA", "Test 2", "Тест 2")]
    [InlineData("ExceptionMessage", "uk-UA", "Exception {0}", "Помилка {0}")]
    public void UsingCulture_WhenSpecified_ThenLocalizerUsesSpecificCultureInScope(string key, string culture, string expectedWithDefault, string expectedWithUsing)
    {
        var beforeUsing = _localizer[key];

        var withUsing = new LocalizedString(string.Empty, string.Empty);

        using (var _ = _sut.UsingCulture(culture))
        {
            withUsing = _localizer[key];
        }

        var afterUsing = _localizer[key];

        beforeUsing.Value.Should().Be(expectedWithDefault);
        withUsing.Value.Should().Be(expectedWithUsing);
        afterUsing.Value.Should().Be(expectedWithDefault);
    }

    [Theory]
    [InlineData("en-US", "Message{0}", "Message {0}", false)]
    [InlineData("en-US", "Test", "Test", false)]
    [InlineData("en-US", "Test1", "Test 1", false)]
    [InlineData("en-US", "Test2", "Test 2", false)]
    [InlineData("uk-UA", "Message{0}", "Повідомлення {0}", false)]
    [InlineData("uk-UA", "Test", "Тест", false)]
    [InlineData("uk-UA", "Test1", "Тест 1", false)]
    [InlineData("uk-UA", "Test2", "Тест 2", false)]
    [InlineData("de", "Test", "Test", false)]
    [InlineData("fr", "Test", "Test", false)]
    public void UsingCulture_WhenCultureSpecified_ThenUsesItOrDefaultForLocalization(string culture, string key, string expectedValue, bool resourceNotFound)
    {
        using var _ = _sut.UsingCulture(culture);

        var value = _localizer[key];

        value.Name.Should().Be(key);
        value.Value.Should().Be(expectedValue);
        value.ResourceNotFound.Should().Be(resourceNotFound);
    }
}
