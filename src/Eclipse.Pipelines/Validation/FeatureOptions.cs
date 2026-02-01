namespace Eclipse.Pipelines.Validation;

internal sealed record FeatureOptions
{
    public bool IsActive { get; init; }

    public required string ErrorMessage { get; init; }

    public List<long> UserIds { get; init; } = [];
}
