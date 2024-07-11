using Eclipse.Localization.Culture;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CurrentCultureTests
{
    private readonly IServiceProvider _serviceProvider;

    private IStringLocalizerFactory Localizer => _serviceProvider.GetRequiredService<IStringLocalizerFactory>();
    private ICurrentCulture Sut => _serviceProvider.GetRequiredService<ICurrentCulture>();

    public CurrentCultureTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddLocalizationV2(options =>
            {
                options.AddJsonFiles("Resources");
                options.DefaultCulture = "en";
            })
            .BuildServiceProvider();
    }

    [Theory]
    [InlineData("Test", "uk", "Test", "Тест")]
    [InlineData("Test1", "uk", "Test 1", "Тест 1")]
    [InlineData("Test2", "uk", "Test 2", "Тест 2")]
    [InlineData("ExceptionMessage", "uk", "Exception {0}", "Помилка {0}")]
    public void UsingCulture_WhenSpecified_ThenLocalizerUsesSpecificCultureInScope(string key, string culture, string expectedWithDefault, string expectedWithUsing)
    {
        var testBeforeUsing = Localizer.Create(GetType())[key];

        var testWithUsing = new LocalizedString(string.Empty, string.Empty);

        using (var _ = Sut.UsingCulture(culture))
        {
            testWithUsing = Localizer.Create(GetType())[key];
        }

        var testAfterUsing = Localizer.Create(GetType())[key];

        testBeforeUsing.Value.Should().Be(expectedWithDefault);
        testWithUsing.Value.Should().Be(expectedWithUsing);
        testAfterUsing.Value.Should().Be(expectedWithDefault);
    }
}
