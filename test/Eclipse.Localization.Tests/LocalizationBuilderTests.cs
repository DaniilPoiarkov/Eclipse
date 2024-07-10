using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;

using FluentAssertions;

using Xunit;

namespace Eclipse.Localization.Tests;

public sealed class LocalizationBuilderTests
{
    private readonly LocalizationBuilder _builder;

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

    [Theory]
    [InlineData("empty.json")]
    [InlineData("invalid.json")]
    public void AddJsonFile_WhenFileIsWrongFormat_ThenExceptionThrown(string file)
    {
        var action = () => _builder.AddJsonFile($"Resources/Invalid/{file}");
        action.Should().Throw<UnableToParseLocalizationResourceException>();
    }

    [Fact]
    public void AddJsonFiles_WhenMultipleJsonFilesWithSameCulture_ThenCombinedTogether()
    {
        _builder.DefaultCulture = "en";
        _builder.AddJsonFiles("Resources/Valid");

        var localizer = _builder.Build();

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
        _builder.DefaultCulture = "en";

        _builder.AddJsonFiles("Resources");

        var localizer = _builder.Build();

        var strings = new List<string>(6);

        for (var i = 1; i <= 4; i++)
        {
            strings.Add(localizer[$"Test{i}"]);
        }

        strings.Add(localizer["Test"]);
        strings.Add(localizer["Arg"]);

        var result = string.Join(" ", strings);

        result.Should().Be("Test 1 Test 2 Test 3 Test 4 Test Arg");
    }
}
