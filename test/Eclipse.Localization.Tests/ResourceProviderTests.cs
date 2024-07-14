using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class ResourceProviderTests
{
    private readonly LocalizationBuilder _builder;

    private readonly Lazy<IResourceProvider> _lazy;

    private IResourceProvider Sut => _lazy.Value;

    public ResourceProviderTests()
    {
        _builder = new LocalizationBuilder
        {
            DefaultCulture = "en"
        };

        _lazy = new(() => new ResourceProvider(Options.Create(_builder)));
    }

    [Theory]
    [InlineData("empty.json")]
    [InlineData("invalid.json")]
    public void AddJsonFile_WhenFileIsWrongFormat_ThenExceptionThrown(string file)
    {
        var action = () => Sut.Get("en", $"Resources/Invalid/{file}");
        action.Should().ThrowExactly<UnableToParseLocalizationResourceException>();
    }
}
