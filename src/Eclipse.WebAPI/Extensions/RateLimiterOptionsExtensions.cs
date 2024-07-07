using Eclipse.WebAPI.Constants;

using Microsoft.AspNetCore.RateLimiting;

using System.Threading.RateLimiting;

namespace Eclipse.WebAPI.Extensions;

public static class RateLimiterOptionsExtensions
{
    public static RateLimiterOptions AddIpAddressSlidingWindow(this RateLimiterOptions options, TimeSpan window, int segmentsPerWidnow, int permitLimit)
    {
        return options.AddPolicy(RateLimiterPolicies.IpAddress, context =>
            RateLimitPartition.GetSlidingWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString(),
                ipAddress => new SlidingWindowRateLimiterOptions
                {
                    Window = window,
                    SegmentsPerWindow = segmentsPerWidnow,
                    PermitLimit = permitLimit,
                })
        );
    }
}
