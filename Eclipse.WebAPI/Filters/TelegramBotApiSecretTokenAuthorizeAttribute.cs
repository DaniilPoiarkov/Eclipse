using Microsoft.AspNetCore.Mvc.Filters;

namespace Eclipse.WebAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TelegramBotApiSecretTokenAuthorizeAttribute : ApiKeyAuthorizeBaseAttribute
{
    public TelegramBotApiSecretTokenAuthorizeAttribute()
        : base("X-Telegram-Bot-Api-Secret-Token") { }

    protected override string GetExpectedValue(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        return configuration["Telegram:SecretToken"]!;
    }
}
