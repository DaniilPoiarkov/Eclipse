using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CurrentCultureTests
{
    private readonly IStringLocalizer<CurrentCultureTests> _localizer;

    private readonly ICurrentCulture _sut;

    public CurrentCultureTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLocalizationV2(options =>
            {
                options.AddJsonFiles("Resources");
                options.DefaultCulture = "en";
            })
            .BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();

        _sut = scope.ServiceProvider.GetRequiredService<ICurrentCulture>();

        var factory = scope.ServiceProvider.GetRequiredService<JsonStringLocalizerFactory>();
        //factory.SetCurrentCulture(_sut);

        _localizer = new TypedJsonStringLocalizer<CurrentCultureTests>(factory);
    }

    [Theory]
    [InlineData("Test", "uk", "Test", "Тест")]
    [InlineData("Test1", "uk", "Test 1", "Тест 1")]
    [InlineData("Test2", "uk", "Test 2", "Тест 2")]
    [InlineData("ExceptionMessage", "uk", "Exception {0}", "Помилка {0}")]
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
}
