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
        var expected = new List<CultureInfo>()
        {
            new()
            {
                Code = "en",
                Culture = "This is english"
            },
            new()
            {
                Code = "fr",
                Culture = "This is franch"
            },
        };

        var english = "English";
        var franch = "Franch";

        LocalizerBuilder<ConfigurationService>.Create(_localizer)
            .For(english)
                .Return("This is english")
            .For(franch)
                .Return("This is franch")
            .Build();

        _options.Value.Returns(
        [
            new()
            {
                Code = "en",
                Culture = english
            },
            new()
            {
                Code = "fr",
                Culture = franch
            }
        ]);

        var result = _sut.GetCultures();

        result.Should().BeEquivalentTo(expected);
    }
}
