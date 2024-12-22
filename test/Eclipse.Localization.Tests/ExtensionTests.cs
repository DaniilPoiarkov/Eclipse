using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using NSubstitute;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class ExtensionTests
{
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
    [InlineData("Test", "Тест")]
    [InlineData("Test 1", "Тест 1")]
    [InlineData("Test 2", "Тест 2")]
    public void ToLocalizableString_WhenDifferentValuesWithSameKey_ThenSameKeyReturned(string left, string right)
    {
        var leftKey = _sut.ToLocalizableString(left);
        var rightKey = _sut.ToLocalizableString(right);

        leftKey.Should().Be(rightKey);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Prüfen")]
    [InlineData("Message with no information provided")]
    [InlineData("!@#5569-=-03123")]
    public void ToLocalizableString_WhenLocalizationNotExist_ThenExceptionThrown(string value)
    {
        var action = () => _sut.ToLocalizableString(value);

        action.Should().ThrowExactly<LocalizationNotFoundException>();
    }

    [Theory]
    [InlineData("Test")]
    public void ToLocalizableString_WhenNotSupported_ThenExceptionThrown(string value)
    {
        var localizer = Substitute.For<IStringLocalizer>();
        var action = () => localizer.ToLocalizableString(value);
        action.Should().ThrowExactly<InvalidOperationException>();
    }
}
