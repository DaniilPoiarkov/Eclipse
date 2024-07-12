using Eclipse.Localization.Culture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CurrentCultureTests
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IStringLocalizer<CurrentCultureTests> _localizer;

    private readonly ICurrentCulture _sut;

    public CurrentCultureTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddLocalizationV2(options =>
            {
                options.AddJsonFiles("Resources");
                options.DefaultCulture = "en";
            })
            .BuildServiceProvider();

        _localizer = _serviceProvider.GetRequiredService<IStringLocalizer<CurrentCultureTests>>();
        _sut = _serviceProvider.GetRequiredService<ICurrentCulture>();
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
