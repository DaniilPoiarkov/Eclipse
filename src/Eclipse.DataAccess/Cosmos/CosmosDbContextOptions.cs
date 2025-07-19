using System.ComponentModel.DataAnnotations;

namespace Eclipse.DataAccess.Cosmos;

public sealed class CosmosDbContextOptions
{
    [Required]
    public required string DatabaseId { get; set; }

    [Required]
    public required string Endpoint { get; set; }

    [Required]
    public required string Container { get; set; }
}
