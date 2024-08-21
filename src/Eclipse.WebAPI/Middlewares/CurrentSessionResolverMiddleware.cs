using Eclipse.WebAPI.Session;

namespace Eclipse.WebAPI.Middlewares;

public sealed class CurrentSessionResolverMiddleware : IMiddleware
{
    private readonly CurrentSession _currentSession;

    public CurrentSessionResolverMiddleware(CurrentSession currentSession)
    {
        _currentSession = currentSession;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return next(context);
        }

        _currentSession.Initialize(context.User);

        return next(context);
    }
}
