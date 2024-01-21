using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using FluentAssertions;

using Xunit;

namespace Eclipse.Localization.Tests;

public class LocalizationBuilderTests
{
    private readonly ILocalizationBuilder _builder;

    public LocalizationBuilderTests()
    {
        _builder = new LocalizationBuilder();
    }

    [Fact]
    public void AddJsonFile_WhenFileNotExist_ThenExceptionThrows()
    {
        var action = () => _builder.AddJsonFile("de.json");
        action.Should().Throw<LocalizationFileNotExistException>();
    }

    [Fact]
    public void AddJsonFile_WhenFileIsWrongFormat_ThenExceptionThrown()
    {
        var action = () => _builder.AddJsonFile("Localization/invalid.json");
        action.Should().Throw<UnableToParseLocalizationResourceException>();
    }

    [Fact]
    public void AddJsonFiles_WhenMultipleJsonFilesWithSameCulture_ThenCombinedTogether()
    {
        _builder.DefaultLocalization = "en";

        var localizer = _builder.AddJsonFiles("Resources")
            .Build();

        var strings = new List<string>(4);

        for (var i = 1; i <= 4; i++)
        {
            strings.Add(localizer[$"Test{i}"]);
        }

        var result = string.Join(' ', strings);
        result.Should().Be("Test 1 Test 2 Test 3 Test 4");
    }

    [Fact]
    public void AddJsonFiles_WhenAddingInfoFromDifferentDirectories_AndHaveSameCultures_ThenCombinedTogether()
    {
        _builder.DefaultLocalization = "en";

        var localizer = _builder.AddJsonFiles("Resources")
            .AddJsonFiles("Localization")
            .Build();

        var strings = new List<string>(6);

        for (var i = 1; i <= 4; i++)
        {
            strings.Add(localizer[$"Test{i}"]);
        }

        strings.Add(localizer["Test"]);
        strings.Add(localizer["ExArg"]);

        var result = string.Join(" ", strings);

        result.Should().Be("Test 1 Test 2 Test 3 Test 4 Test ExArg");
    }
}
