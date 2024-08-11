using Eclipse.WebAPI.Models;

using Microsoft.Extensions.Localization;

using Newtonsoft.Json;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ErrorLocalizerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using var stream = new MemoryStream();

        var oridignal = context.Response.Body;

        context.Response.Body = stream;

        await next(context);

        stream.Position = 0;

        if (context.Response.StatusCode < 400)
        {
            await stream.CopyToAsync(oridignal);
            context.Response.Body = oridignal;
            return;
        }

        using var reader = new StreamReader(context.Response.Body);

        var body = await reader.ReadToEndAsync();

        var problems = JsonConvert.DeserializeObject<ProblemsResponse>(body);

        if (problems is null || problems.Errors is not { Length: > 0 })
        {
            await stream.CopyToAsync(oridignal);
            context.Response.Body = oridignal;
            return;
        }

        var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<ErrorLocalizerMiddleware>>();

        var error = problems.Errors[0];
        problems.Errors = null;

        problems.Detail = localizer[error.Description, error.Args];

        using var writer = new StreamWriter(oridignal);

        await writer.WriteAsync(JsonConvert.SerializeObject(problems));

        context.Response.Body = oridignal;
    }
}
