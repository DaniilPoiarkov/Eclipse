using System.ComponentModel.DataAnnotations;

namespace Eclipse.Common.Telegram;

public sealed class TelegramOptions
{
    [Required]
    public string Token { get; set; } = null!;

    public long Chat { get; set; }
}
