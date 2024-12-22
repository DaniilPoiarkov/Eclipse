using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using NSubstitute;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class CultureResolverMiddlewareTests
{
    private readonly IOptions<LocalizationBuilder> _options;

    private readonly CultureResolverMiddleware _sut;

    public CultureResolverMiddlewareTests()
    {
        var builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        _options = Options.Create(builder);

        _sut = new CultureResolverMiddleware(_options);
    }

    [Fact]
    public async Task InvokeAsync_WhenCultureNotSpecified_ThenDefaultCultureUsed()
    {
        var context = Substitute.For<HttpContext>();
        var next = Substitute.For<RequestDelegate>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider.GetService(typeof(IEnumerable<ICultureResolver>))
            .Returns(new List<ICultureResolver>());

        context.RequestServices.Returns(serviceProvider);

        await _sut.InvokeAsync(context, next);

        serviceProvider.Received().GetService(typeof(IEnumerable<ICultureResolver>));
    }

    [Theory]
    [InlineData("en", 1)]
    public async Task InvokeAsync_WhenCultureSpecified_ThenUseIt(string culture, int expectedCalls)
    {
        var context = Substitute.For<HttpContext>();
        var next = Substitute.For<RequestDelegate>();
        var serviceProvider = Substitute.For<IServiceProvider>();

        var resolver = new TestResolver(culture);

        serviceProvider.GetService(typeof(IEnumerable<ICultureResolver>))
            .Returns(new List<ICultureResolver> { resolver });

        context.RequestServices.Returns(serviceProvider);

        await _sut.InvokeAsync(context, next);

        serviceProvider.Received().GetService(typeof(IEnumerable<ICultureResolver>));
        resolver.Count.Should().Be(expectedCalls);
    }

    private class TestResolver : ICultureResolver
    {
        private readonly string _culture;

        internal int Count { get; private set; }

        public TestResolver(string culture)
        {
            _culture = culture;
        }

        public bool TryGetCulture(HttpContext context, [NotNullWhen(true)] out CultureInfo? cultureInfo)
        {
            Count++;
            cultureInfo = CultureInfo.GetCultureInfo(_culture);
            return true;
        }
    }
}
