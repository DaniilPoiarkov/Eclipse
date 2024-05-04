using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Telegram.Commands;

[Serializable]
public sealed class CommandDto
{
    [Required]
    public string Command { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
