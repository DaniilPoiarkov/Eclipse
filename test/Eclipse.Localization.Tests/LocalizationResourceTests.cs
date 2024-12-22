using Eclipse.Localization.Resources;

using FluentAssertions;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class LocalizationResourceTests
{
    [Theory]
    [InlineData("en", "Test", "Test")]
    public void ToString_WhenCalled_ThenListsCultureAndTexts(string culture, string key, string value)
    {
        var resource = new LocalizationResource
        {
            Culture = culture,
            Texts = new()
            {
                [key] = value
            }
        };

        var result = resource.ToString();

        var expected = $"Culture: {culture};{Environment.NewLine}" +
            $"Texts:{Environment.NewLine}" +
            $"{string.Join(", ", resource.Texts.Select(pair => $"{pair.Key} - {pair.Value}"))}";

        result.Should().Be(expected);
    }
}
