using Eclipse.Localization.Culture.Resolvers;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using NSubstitute;

using Xunit;

namespace Eclipse.Localization.Tests.Resolvers;

public sealed class HeaderCultureResolverTests
{
    private readonly HeaderCultureResolver _sut = new();

    [Theory]
    [InlineData("en")]
    [InlineData("uk")]
    public void TryGetCulture_WhenCultureCanBeResolved_ThenReturnsTrueAndCulture(string culture)
    {
        var context = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();

        var headers = new HeaderDictionary
        {
            ["Content-Language"] = culture,
        };

        request.Headers.Returns(headers);
        context.Request.Returns(request);

        var result = _sut.TryGetCulture(context, out var cultureInfo);

        result.Should().BeTrue();
        cultureInfo.Should().NotBeNull();
        cultureInfo!.Name.Should().Be(culture);
    }

    [Fact]
    public void TryGetCulture_WhenNoHeadersProvided_ThenReturnsFalse()
    {
        var context = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();

        request.Headers.Returns(new HeaderDictionary());
        context.Request.Returns(request);

        var result = _sut.TryGetCulture(context, out var cultureInfo);

        result.Should().BeFalse();
        cultureInfo.Should().BeNull();
    }

    [Fact]
    public void TryGetCulture_WhenHeaderWithoutValueProvided_ThenReturnsFalse()
    {
        var context = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();

        var headers = new HeaderDictionary
        {
            ["Content-Language"] = string.Empty,
        };

        request.Headers.Returns(headers);
        context.Request.Returns(request);

        var result = _sut.TryGetCulture(context,out var cultureInfo);
        
        result.Should().BeFalse();
        cultureInfo.Should().BeNull();
    }
}
