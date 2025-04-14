using Microsoft.AspNetCore.Mvc.Formatters;

using System.Text;
using System.Text.Json;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.WebAPI.Formatters;

internal sealed class TelegramBotInputFormatter : TextInputFormatter
{
    public TelegramBotInputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedMediaTypes.Add("application/json");
    }

    protected override bool CanReadType(Type type) => type == typeof(Update);

    public sealed override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var model = await JsonSerializer.DeserializeAsync(
            context.HttpContext.Request.Body,
            context.ModelType,
            JsonBotAPI.Options,
            context.HttpContext.RequestAborted
        );

        return await InputFormatterResult.SuccessAsync(model);
    }
}
