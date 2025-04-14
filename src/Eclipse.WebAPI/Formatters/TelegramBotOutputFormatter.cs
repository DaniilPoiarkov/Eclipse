using Microsoft.AspNetCore.Mvc.Formatters;

using System.Text;
using System.Text.Json;

using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;

namespace Eclipse.WebAPI.Formatters;

internal sealed class TelegramBotOutputFormatter : TextOutputFormatter
{
    public TelegramBotOutputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedMediaTypes.Add("application/json");
    }

    protected override bool CanWriteType(Type? type) => typeof(IRequest).IsAssignableFrom(type);

    public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var stream = context.HttpContext.Response.Body;
        await JsonSerializer.SerializeAsync(stream, context.Object, JsonBotAPI.Options, context.HttpContext.RequestAborted);
    }
}
