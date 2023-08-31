namespace Eclipse.WebAPI.Services.TelegramServices;

public class SendMessageModel
{
    public string Message { get; set; } = string.Empty;

    public long ChatId { get; set; }

    public string EclipseToken { get; set; } = string.Empty;
}
