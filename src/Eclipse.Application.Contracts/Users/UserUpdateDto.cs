using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Users;

[Serializable]
public sealed class UserUpdateDto
{
    [Required]
    public string? Name { get; set; }

    public string Surname { get; set; } = string.Empty;

    [Required]
    public string UserName { get; set; } = string.Empty;

    public string? Culture { get; set; }

    public bool NotificationsEnabled { get; set; }
}
