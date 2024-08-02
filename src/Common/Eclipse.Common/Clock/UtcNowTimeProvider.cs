
namespace Eclipse.Common.Clock;

public sealed class UtcNowTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.UtcNow;
}
