using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using NSubstitute;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CurrentCultureTests
{
    private readonly IStringLocalizer<CurrentCultureTests> _localizer;

    private readonly CurrentCulture _sut;

    public CurrentCultureTests()
    {
        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        builder.AddJsonFiles("Resources");

        var options = Options.Create(builder);

        var resourceProvider = new ResourceProvider(options);
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _sut = new CurrentCulture(options);

        httpContextAccessor.HttpContext?.RequestServices
            .GetService(typeof(ICurrentCulture))
            .Returns(_sut);

        var factory = new JsonStringLocalizerFactory(options, resourceProvider, httpContextAccessor);

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
