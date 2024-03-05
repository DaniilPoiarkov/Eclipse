using System.ComponentModel.DataAnnotations;

namespace Eclipse.Telegram.Application.Contracts.Commands;

[Serializable]
public sealed class CommandDto
{
    [Required]
    public string Command { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
