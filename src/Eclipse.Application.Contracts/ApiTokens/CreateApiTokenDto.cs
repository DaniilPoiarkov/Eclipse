using System.ComponentModel.DataAnnotations;

namespace Eclipse.Application.Contracts.ApiTokens;

public sealed class CreateApiTokenDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
}
