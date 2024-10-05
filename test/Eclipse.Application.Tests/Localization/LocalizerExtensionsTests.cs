using Eclipse.Application.Localizations;
using Eclipse.Common.Results;
using Eclipse.Tests.Builders;

using FluentAssertions;

using Microsoft.Extensions.Localization;

using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

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

    [Fact]
    public void LocalizeError_WhenResourceNotExist_ThenDescriptionReturned()
    {
        var error = Error.Failure("Test", "Test:Error");

        var localizer = LocalizerBuilder<LocalizerExtensionsTests>.Create().Build();

        localizer.Configure()["Test:Error", []]
            .Returns(new LocalizedString("Test:Error", "", true));

        var actual = localizer.LocalizeError(error);

        actual.Should().Be(error.Description);
    }

    [Fact]
    public void LocalizeError_WhenExceptionThrown_ThenDescriptionReturned()
    {
        var error = Error.Failure("Test", "Test:Error");

        var localizer = LocalizerBuilder<LocalizerExtensionsTests>.Create().Build();

        localizer.Configure()["Test:Error"].Throws<Exception>();

        var actual = localizer.LocalizeError(error);

        actual.Should().Be(error.Description);
    }
}
