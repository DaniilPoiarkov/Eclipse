using System.ComponentModel.DataAnnotations;

namespace Eclipse.Infrastructure.Google;

public class GoogleOptions
{
    [Required]
    public string Credentials { get; set; } = null!;
}
