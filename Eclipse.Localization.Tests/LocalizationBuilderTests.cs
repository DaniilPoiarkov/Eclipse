using Eclipse.Localization.Builder;

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
}
