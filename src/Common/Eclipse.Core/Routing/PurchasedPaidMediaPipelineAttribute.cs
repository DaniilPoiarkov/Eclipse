namespace Eclipse.Core.Routing;

/// <summary>
/// Pipeline for processing PurchasedPaidMedia updates.
/// Keep in mind that only one pipeline with this attribute should be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PurchasedPaidMediaPipelineAttribute : Attribute
{

}
