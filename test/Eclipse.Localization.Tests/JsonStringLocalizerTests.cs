using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class JsonStringLocalizerTests
{
    private readonly LocalizationResource _resource;

    private readonly IStringLocalizer _sut;

    public JsonStringLocalizerTests()
    {
        _resource = new LocalizationResource
        {
            Culture = "en",
            Texts = new Dictionary<string, string>
            {
                ["Test"] = "Test",
                ["Arg"] = "Argument",
                ["Message{0}"] = "Message {0}",
                ["Message{0}{1}"] = "Message {0} with {1}"
            }
        };

        _sut = new JsonStringLocalizer(_resource);
    }

    [Theory]
    [InlineData("Test", "Test", false)]
    [InlineData("Arg", "Argument", false)]
    [InlineData("Message{0}", "Message {0}", false)]
    [InlineData("Message{0}{1}", "Message {0} with {1}", false)]
    [InlineData("NotFound", "NotFound", true)]
    [InlineData("OneMoreNotFound", "OneMoreNotFound", true)]
    public void GetString_WhenCalledWithBrackets_ThenProperValueReturned(string key, string expected, bool resourceNotFound)
    {
        var actual = _sut[key];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
        actual.SearchedLocation.Should().BeNull();
    }

    [Theory]
    [InlineData("Message{0}", "Message info", false, "info")]
    [InlineData("Message{0}{1}", "Message info with 5", false, "info", 5)]
    [InlineData("Message{0}{1}", "Message info with 5", false, "info", 5, "true", "!@#", 09)]
    [InlineData("{0}Message", "{0}Message", true, "info")]
    [InlineData("{0}Message{1}", "{0}Message{1}", true, "info", "4", 5)]
    public void GetString_WhenCalledWithArguments_ThenFormatterStringReturned(string key, string expected, bool resourceNotFound, params object[] arguments)
    {
        var actual = _sut[key, arguments];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
    }

    [Fact]
    public void GetAllStrings_WhenCalled_ThenAllValuesReturned()
    {
        var actual = _sut.GetAllStrings().ToArray();

        actual.All(s => s.ResourceNotFound).Should().BeFalse();

        actual.Length.Should().Be(_resource.Texts.Count);

        var keysFromResource = _resource.Texts.Select(t => t.Key);
        var keysFromActual = actual.Select(s => s.Name);

        keysFromResource.Except(keysFromActual).Should().BeEmpty();

        var valuesFromResource = _resource.Texts.Select(t => t.Value);
        var valuesFromActual = actual.Select(s => s.Value);

        valuesFromResource.Except(valuesFromActual).Should().BeEmpty();
    }
}
