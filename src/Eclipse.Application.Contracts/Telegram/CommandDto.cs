namespace Eclipse.Application.Contracts.Telegram;

[Serializable]
public sealed record CommandDto(string Command, string Description);
