using System.ComponentModel.DataAnnotations;

namespace Eclipse.DataAccess.CosmosDb;

public sealed class CosmosDbContextOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;

    [Required]
    public string DatabaseId { get; set; } = null!;

    [Required]
    public string Endpoint { get; set; } = null!;
}
