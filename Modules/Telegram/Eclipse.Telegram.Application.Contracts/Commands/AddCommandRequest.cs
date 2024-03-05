namespace Eclipse.Telegram.Application.Contracts.Commands;

[Serializable]
public sealed class AddCommandRequest
{
    public string? Command { get; set; }
    public string? Description { get; set; }
}