namespace Eclipse.Application.Contracts.Telegram;

[Serializable]
public sealed record AddCommandRequest(string? Command, string? Description);
