using Eclipse.Application.Localizations;
using Eclipse.Common.Results;
using Eclipse.Tests.Builders;

using FluentAssertions;

using Xunit;

namespace Eclipse.Application.Tests.Localization;

public sealed class LocalizerExtensionsTests
{
    [Fact]
    public void LocalizeError_WhenCalled_ThenLocalizedErrorReturned()
    {
        var localizer = LocalizerBuilder<LocalizerExtensionsTests>.Create()
            .ForWithArgs("Test:Error")
            .Return("Test error")
            .Build();

        var error = Error.Failure("Test", "Test:Error");

        var actual = localizer.LocalizeError(error);

        actual.Should().Be("Test error");
    }
}
