using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Telegram.Commands;

public class CommandDto
{
    [Required]
    public string Command { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
