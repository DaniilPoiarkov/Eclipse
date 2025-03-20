using Microsoft.Extensions.Logging;

using NSubstitute;

namespace Eclipse.Tests.Extensions;

public static class LoggerExtensions
{
    public static void ShouldReceiveLog(this ILogger logger, LogLevel logLevel)
    {
        logger.Received().Log<object>(
            logLevel,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}
