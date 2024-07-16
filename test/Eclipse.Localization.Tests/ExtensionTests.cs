using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class ExtensionTests
{
    private readonly ICurrentCulture _currentCulture;

    private readonly IStringLocalizer<ExtensionTests> _sut;

    public ExtensionTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLocalization(options =>
            {
                options.DefaultCulture = "en";
                options.AddJsonFiles("Resources/Valid");
            })
            .BuildServiceProvider();

        _currentCulture = Substitute.For<ICurrentCulture>();
        _sut = serviceProvider.GetRequiredService<IStringLocalizer<ExtensionTests>>();
    }

    [Theory]
    [InlineData("Test", "Test")]
    [InlineData("Test 1", "Test1")]
    [InlineData("Test 2", "Test2")]
    [InlineData("Тест", "Test")]
    [InlineData("Тест 1", "Test1")]
    [InlineData("Тест 2", "Test2")]
    public void ToLocalizableString_WhenValueValid_ThenReturnsKey(string value, string expectedKey)
    {
        var key = _sut.ToLocalizableString(value);
        key.Should().Be(expectedKey);
    }

    [Theory]
    [InlineData("en", "Message{0}", "Message {0}")]
    [InlineData("uk", "Message{0}", "Повідомлення {0}")]
    public void UseCurrentCulture_WhenCultureSpecified_ThenUsesItForLocalization(string culture, string key, string expectedValue)
    {
        _currentCulture.Culture.Returns(culture);
        _sut.UseCurrentCulture(_currentCulture);

        var value = _sut[key];
        value.Value.Should().Be(expectedValue);
    }
}
