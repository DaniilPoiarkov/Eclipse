using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Provider;

/// <summary>
/// Returns <a cref="PipelineBase"></a> which match specified <![CDATA[route]]>
/// </summary>
[Obsolete($"Replace with {nameof(IPipelineProvider)}")]
public interface ILegacyPipelineProvider
{
    PipelineBase Get(string route);
}
