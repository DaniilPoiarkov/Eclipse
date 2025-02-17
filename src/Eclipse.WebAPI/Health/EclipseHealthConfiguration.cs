using Eclipse.WebAPI.Constants;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Eclipse.WebAPI.Health;

public static class EclipseHealthConfiguration
{
    public static WebApplication UseEclipseHealthChecks(this WebApplication app)
    {
        var options = new HealthCheckOptions
        {
            AllowCachingResponses = true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };

        app.MapHealthChecks("/_health-checks", options)
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithOpenApi();

        return app;
    }
}
