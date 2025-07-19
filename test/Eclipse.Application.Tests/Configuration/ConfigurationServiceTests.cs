using Eclipse.Application.Configuration;
using Eclipse.Application.Contracts.Configuration;
using Eclipse.Tests.Builders;

using FluentAssertions;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Configuration;

public sealed class ConfigurationServiceTests
{
    private readonly IStringLocalizer<ConfigurationService> _localizer;

    private readonly IOptions<CultureList> _options;

    private readonly ConfigurationService _sut;

    public ConfigurationServiceTests()
    {
        _localizer = Substitute.For<IStringLocalizer<ConfigurationService>>();
        _options = Substitute.For<IOptions<CultureList>>();
        _sut = new(_options, _localizer);
    }

    [Fact]
    public void GetCultures_WhenRequested_ThenReturnsLocalizedResponse()
    {
        var english = "English";
        var franch = "Franch";

        var expectedEnglish = "This is english";
        var expectedFranch = "This is franch";

        _options.Value.Returns(
        [
            new(english, "en"),
            new(franch, "fr")
        ]);

        var expected = new List<CultureInfo>()
        {
            new(expectedEnglish, "en"),
            new(expectedFranch, "fr")
        };

        LocalizerBuilder<ConfigurationService>.Configure(_localizer)
            .For(english)
                .Return(expectedEnglish)
            .For(franch)
                .Return(expectedFranch);

        var result = _sut.GetCultures();

        result.Should().BeEquivalentTo(expected);
    }
}
