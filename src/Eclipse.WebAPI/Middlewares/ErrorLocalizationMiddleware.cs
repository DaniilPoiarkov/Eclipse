using Eclipse.WebAPI.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Newtonsoft.Json;

namespace Eclipse.WebAPI.Middlewares;

public sealed class ErrorLocalizationMiddleware : IMiddleware
{
    private readonly IStringLocalizer<ErrorLocalizationMiddleware> _localizer;

    private readonly ILogger<ErrorLocalizationMiddleware> _logger;

    public ErrorLocalizationMiddleware(
        IStringLocalizer<ErrorLocalizationMiddleware> localizer,
        ILogger<ErrorLocalizationMiddleware> logger)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        using var ms = new MemoryStream();

        var original = context.Response.Body;

        context.Response.Body = ms;

        await next(context);

        if (context.Response.StatusCode is < StatusCodes.Status400BadRequest or StatusCodes.Status401Unauthorized)
        {
            await WriteToOriginalBodyAsync(context, ms, original);
            return;
        }

        try
        {
            await RewriteBodyAsync(context, ms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot localize error response. TraceId: {traceId}", context.TraceIdentifier);
        }

        await WriteToOriginalBodyAsync(context, ms, original);
    }

    private static async Task WriteToOriginalBodyAsync(HttpContext context, MemoryStream ms, Stream original)
    {
        ms.Position = 0;
        await ms.CopyToAsync(original);
        context.Response.Body = original;
    }

    private async Task RewriteBodyAsync(HttpContext context, MemoryStream ms)
    {
        ms.Position = 0;

        string body = await ReadBodyAsync(ms);
        string json = GetLocalizedJson(body);

        ms.SetLength(0);

        await RewriteBodyAsync(ms, json);

        context.Response.ContentLength = ms.Length;
    }

    private static async Task RewriteBodyAsync(Stream ms, string json)
    {
        using var writer = new StreamWriter(ms, leaveOpen: true);

        await writer.WriteAsync(json);
        await writer.FlushAsync();
    }

    private static async Task<string> ReadBodyAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);

        return await reader.ReadToEndAsync();
    }

    private string GetLocalizedJson(string body)
    {
        var response = JsonConvert.DeserializeObject<ProblemsResponse>(body);

        if (response is null || response.Error is null)
        {
            return body;
        }

        var problems = new ProblemDetails
        {
            Type = response.Type,
            Title = response.Title,
            Status = response.Status,
            Instance = response.Instance,

            Detail = _localizer[response.Error.Description, response.Error.Args],
        };

        return JsonConvert.SerializeObject(problems, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        });
    }
}
