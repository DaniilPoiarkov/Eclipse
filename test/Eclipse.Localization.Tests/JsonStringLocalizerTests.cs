﻿using Eclipse.Localization.Builder;
using Eclipse.Localization.Resources;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class JsonStringLocalizerTests
{
    private readonly IStringLocalizer<JsonStringLocalizerTests> _localizer;

    private static readonly string _file = "Resources/Valid/en.json";

    private static readonly string _culture = "en";

    public JsonStringLocalizerTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLocalizationV2(options =>
            {
                options.DefaultCulture = "en";
                options.AddJsonFile("Resources/Valid/en.json");
            })
            .BuildServiceProvider();

        _localizer = serviceProvider.GetRequiredService<IStringLocalizer<JsonStringLocalizerTests>>();
    }

    [Theory]
    [InlineData("Test", "Test", false)]
    [InlineData("Arg", "Argument", false)]
    [InlineData("{0}Message{1}", "{0} message {1}", false)]
    [InlineData("Message{0}{1}", "Message {0} with {1}", false)]
    [InlineData("NotFound", "NotFound", true)]
    [InlineData("OneMoreNotFound", "OneMoreNotFound", true)]
    public void GetString_WhenCalledWithBrackets_ThenProperValueReturned(string key, string expected, bool resourceNotFound)
    {
        var actual = _localizer[key];

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
    [InlineData("{0}Message{1}{2}", "{0}Message{1}{2}", true, "info", "4", 5, 42)]
    public void GetString_WhenCalledWithArguments_ThenFormatterStringReturned(string key, string expected, bool resourceNotFound, params object[] arguments)
    {
        var actual = _localizer[key, arguments];

        actual.Value.Should().Be(expected);
        actual.Name.Should().Be(key);
        actual.ResourceNotFound.Should().Be(resourceNotFound);
    }

    [Fact]
    public void GetAllStrings_WhenCalled_ThenAllValuesReturned()
    {
        var builder = new LocalizationBuilderV2();
        builder.AddJsonFile(_file);
        var resourceProvider = new ResourceProvider(Options.Create(builder));

        var resource = resourceProvider.Get(_culture, _file);

        var actual = _localizer.GetAllStrings().ToArray();

        actual.All(s => s.ResourceNotFound).Should().BeFalse();

        actual.Length.Should().Be(resource.Texts.Count);

        var keysFromResource = resource.Texts.Select(t => t.Key);
        var keysFromActual = actual.Select(s => s.Name);

        keysFromResource.Except(keysFromActual).Should().BeEmpty();

        var valuesFromResource = resource.Texts.Select(t => t.Value);
        var valuesFromActual = actual.Select(s => s.Value);

        valuesFromResource.Except(valuesFromActual).Should().BeEmpty();
    }
}