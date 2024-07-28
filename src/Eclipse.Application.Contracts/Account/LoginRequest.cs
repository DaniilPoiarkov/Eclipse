using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.Account;

public sealed class LoginRequest
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string SignInCode { get; set; } = null!;
}
