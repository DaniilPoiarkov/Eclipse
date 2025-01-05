using Eclipse.Application.Url;

using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Url;

public sealed class AppUrlProviderTests
{
    private readonly IConfiguration _configuration;

    private readonly AppUrlProvider _sut;

    public AppUrlProviderTests()
    {
        _configuration = Substitute.For<IConfiguration>();

        _sut = new AppUrlProvider(_configuration);
    }

    [Theory]
    [InlineData("https://test.com")]
    public void AppUrl_WhenAccessed_ThenReturnsUrl(string url)
    {
        var section = Substitute.For<IConfigurationSection>();
        section.GetSection("SelfUrl").Value.Returns(url);

        _configuration.GetSection("App").Returns(section);

        _sut.AppUrl.Should().Be(url);
    }

    [Fact]
    public void AppUrl_WhenConfigNotProvided_ThenExceptionThrown()
    {
        var section = Substitute.For<IConfigurationSection>();
        section.GetSection("SelfUrl").Value.Returns((string)null!);

        _configuration.GetSection("App").Returns(section);

        var action = () => _ = _sut.AppUrl;

        action.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("App.SelfUrl configuration is not provided.");
    }
}
