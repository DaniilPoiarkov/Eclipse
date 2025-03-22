using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserUpdateDto
{
    [Required]
    public required string Name { get; init; }

    public string Surname { get; init; } = string.Empty;

    [Required]
    public required string UserName { get; init; }

    public string? Culture { get; init; }

    public bool NotificationsEnabled { get; init; }

    public bool IsEnabled { get; init; }
}
